namespace Fanex.NCB.BotService.EndPoint.Controllers.Telegram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Enum;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageController : Controller
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<MessageController> filelogger;
        private readonly Logging.ILogger logger;
        private readonly IConversationService conversationService;
        private readonly ITelegramBotService botService;
        private readonly IMessageLogService messageLogService;

        public MessageController(
            IOptions<AppSettings> appSettings,
            ILogger<MessageController> filelogger,
            Logging.ILogger logger,
            IConversationService conversationService,
            ITelegramBotService botService,
            IMessageLogService messageLogService)
        {
            this.appSettings = appSettings.Value;
            this.filelogger = filelogger;
            this.logger = logger;
            this.conversationService = conversationService;
            this.botService = botService;
            this.messageLogService = messageLogService;
        }

        [HttpPost]
        [Route("api/telegram/sendMessage")]
        public async Task<ObjectResult> SendMessage([FromBody] ConversationInfo conversationInfo)
        {
            var errorMessage = string.Empty;
            try
            {
                if (!appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram))
                {
                    errorMessage = Resources.Resources.DisableChannelErrorMessage;
                }
                else
                {
                    _ = long.TryParse(conversationInfo.BotConversationId, out long botId);
                    _ = long.TryParse(conversationInfo.ConversationId, out long chatId);
                    await botService.SendTextMessageAsync(Guid.NewGuid().ToString(), botId, chatId, conversationInfo.Message.ToMarkdownV2());
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                await WriteLogAsync(ex);
            }

            return Ok(errorMessage);
        }

        [HttpPost]
        [Route("api/telegram/sendMultiMessage")]
        public async Task<ObjectResult> SendMultiMessage([FromBody] IList<NotificationMessage> notificationMessages)
        {
            const short Milliseconds = 1000;
            var results = new List<SendMessageResult>();
            if (notificationMessages != null && notificationMessages.Any())
            {
                string transid = Guid.NewGuid().ToString();
                filelogger.LogWarning($"{transid}-Begin send messages");
                var notifications = conversationService.GetDistinctNotificationConversation(notificationMessages);
                IList<List<ConversationInfo>> batches = DivideConversionByBatch(notifications);
                foreach (var batch in batches)
                {
                    var tasks = new List<Task<SendMessageResult>>();
                    foreach (ConversationInfo conversationInfo in batch)
                    {
                        SendMessage(transid, tasks, conversationInfo);
                    }

                    results.AddRange((await Task.WhenAll(tasks)).ToList());
                    await Task.Delay(Milliseconds);
                }

                filelogger.LogWarning($"{transid}-End send messages");
            }

            return Ok(results);
        }

        private static IList<List<ConversationInfo>> DivideConversionByBatch(IEnumerable<NotificationMessage> notifications)
        {
            var floatConversations = new List<ConversationInfo>();
            var batches = new List<List<ConversationInfo>>();
            foreach (NotificationMessage notification in notifications)
            {
                foreach (ConversationInfo conversationInfo in notification.Recipients)
                {
                    conversationInfo.NotificationId = notification.NotificationId;
                    conversationInfo.Message = notification.Message;
                    floatConversations.Add(conversationInfo);

                    if (!batches.Any()
                        || floatConversations.Count(x => x.ConversationId == conversationInfo.ConversationId
                            && x.BotConversationId == conversationInfo.BotConversationId) > batches.Count)
                    {
                        var batch = new List<ConversationInfo>
                        {
                            conversationInfo
                        };
                        batches.Add(batch);
                    }
                    else
                    {
                        var index = floatConversations.Count(x => x.ConversationId == conversationInfo.ConversationId
                            && x.BotConversationId == conversationInfo.BotConversationId);
                        batches[index - 1].Add(conversationInfo);
                    }
                }
            }

            return batches;
        }

        private void SendMessage(string transid, List<Task<SendMessageResult>> tasks, ConversationInfo conversationInfo)
        {
            bool isEnableTelegramChannel = appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram);
            tasks.Add(!isEnableTelegramChannel ? Task.Run(() => new SendMessageResult
            {
                NotificationId = conversationInfo.NotificationId,
                ConversationId = conversationInfo.ConversationId,
                SentTime = DateTime.UtcNow,
                ErrorCode = 1,
                ErrorMessage = Resources.Resources.DisableChannelErrorMessage,
                IsIgnoreRetry = true
            }) : Task.Run(() => SendMessageAsync(transid, conversationInfo)));
        }

        protected internal virtual async Task<SendMessageResult> SendMessageAsync(string transid, ConversationInfo conversationInfo)
        {
            if (conversationInfo.IsDuplicated)
            {
                return DuplicatedResult(conversationInfo.NotificationId, conversationInfo.ConversationId);
            }

            bool isDuplicated = messageLogService.IsInvalidSendMessageToBotService(conversationInfo.ConversationId, conversationInfo.Message);

            if (isDuplicated)
            {
                return DuplicatedResult(conversationInfo.NotificationId, conversationInfo.ConversationId);
            }

            string guiid = Guid.NewGuid().ToString();
            try
            {
                _ = long.TryParse(conversationInfo.BotConversationId, out long botId);
                _ = long.TryParse(conversationInfo.ConversationId, out long chatId);
                await messageLogService.InsertMessageLogIntoCache(conversationInfo.ConversationId, conversationInfo.Message, guiid);
                SendMessageResult result = await SendTextMessageAsync(transid, botId, chatId, conversationInfo.NotificationId, conversationInfo.Message);
                if (result.ErrorCode != 0)
                {
                    await messageLogService.RemoveMessageLogOnCache(conversationInfo.ConversationId, guiid);
                }

                var thread = new Thread(async () => await AddMessageLogToDatabaseAsync(conversationInfo, conversationInfo.Message, result));
                thread.Start();

                return result;
            }
            catch (Exception exception)
            {
                await messageLogService.RemoveMessageLogOnCache(conversationInfo.ConversationId, guiid);
                await WriteLogAsync(exception);
                return new SendMessageResult
                {
                    NotificationId = conversationInfo.NotificationId,
                    ConversationId = conversationInfo.ConversationId,
                    SentTime = DateTime.UtcNow,
                    ErrorCode = 1,
                    ErrorMessage = exception.Message
                };
            }
        }

        private static SendMessageResult DuplicatedResult(int notificationId, string conversationId)
            => new()
            {
                ConversationId = conversationId,
                SentTime = DateTime.UtcNow,
                ErrorCode = -1,
                ErrorMessage = Resources.Resources.DuplicatedErrorMessage,
                NotificationId = notificationId,
                IsIgnoreRetry = true
            };

        protected internal virtual async Task<SendMessageResult> SendTextMessageAsync(string transid, long botId, long chatId, int notificationId, string messageContent)
        {
            var result = new SendMessageResult
            {
                ConversationId = chatId.ToString(),
                NotificationId = notificationId
            };

            try
            {
                await botService.SendTextMessageAsync(transid, botId, chatId, messageContent.ToMarkdownV2());
            }
            catch (Exception exception)
            {
                var param = new Dictionary<string, object>
                {
                    { "notificationId", notificationId },
                    { "botId", botId },
                    { "chatId", chatId },
                    { "messageContent", messageContent }
                };

                await WriteLogAsync(exception, param);

                result.ErrorCode = 1;
                result.ErrorMessage = exception.Message;

                if (exception.Message.Contains("Message is too long", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.ErrorMessage = Resources.Resources.MessageIsTooLong;
                    result.IsIgnoreRetry = true;
                }

                if (exception.Message.Contains("Forbidden: bot was kicked from the supergroup chat", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.ErrorMessage = Resources.Resources.GroupChatDeleted;
                    result.IsIgnoreRetry = true;
                }
            }

            result.SentTime = DateTime.UtcNow;
            return result;
        }

        protected internal virtual async Task AddMessageLogToDatabaseAsync(ConversationInfo conversationInfo, string messageContent, SendMessageResult result)
        {
            try
            {
                var command = new AddMessageLogCommand
                {
                    FromConversationId = conversationInfo.BotConversationId,
                    ToConversationId = conversationInfo.ConversationId,
                    Content = messageContent,
                    ReferenceId = result.NotificationId,
                    IsSuccessfull = result.ErrorCode == 0,
                    ErrorMessage = result.ErrorMessage
                };

                await conversationService.AddMessageLog(command);
            }
            catch (Exception ex)
            {
                await WriteLogAsync(ex);
            }
        }

        protected internal virtual async Task WriteLogAsync(Exception exception, IDictionary<string, object> param = null)
        {
            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
            if (param != null)
            {
                logMessage.AddSection("Params", param);
            }

            await logger.ErrorAsync(logMessage.ToString());
        }
    }
}