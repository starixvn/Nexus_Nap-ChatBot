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
    using Microsoft.Extensions.Options;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageController : Controller
    {
        private readonly AppSettings appSettings;
        private readonly ILogger logger;
        private readonly IConversationService conversationService;
        private readonly ITelegramBotService botService;
        private readonly IMessageLogService messageLogService;

        public MessageController(
            IOptions<AppSettings> appSettings,
            ILogger logger,
            IConversationService conversationService,
            ITelegramBotService botService,
            IMessageLogService messageLogService)
        {
            this.appSettings = appSettings.Value;
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
                    await botService.SendTextMessageAsync(botId, chatId, conversationInfo.Message.ToMarkdownV2());
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
            var results = new List<SendMessageResult>();
            var tasks = new List<Task<SendMessageResult>>();
            var conversations = new List<ConversationInfo>();
            if (notificationMessages != null && notificationMessages.Any())
            {
                var notifications = conversationService.GetDistinctNotificationConversation(notificationMessages);
                foreach (NotificationMessage notification in notifications)
                {
                    foreach (ConversationInfo conversationInfo in notification.Recipients)
                    {
                        await SendMessageAsync(tasks, conversations, conversationInfo, notification);
                    }
                }

                results = (await Task.WhenAll(tasks)).ToList();
            }

            return Ok(results);
        }

        private async Task SendMessageAsync(
            List<Task<SendMessageResult>> tasks,
            List<ConversationInfo> conversations,
            ConversationInfo conversationInfo,
            NotificationMessage notificationMessage)
        {
            const short Milliseconds = 1000;
            if (conversations.Any(x => x.ConversationId == conversationInfo.ConversationId
                && x.BotConversationId == conversationInfo.BotConversationId))
            {
                await Task.Delay(Milliseconds);
            }
            else
            {
                conversations.Add(new ConversationInfo
                {
                    ConversationId = conversationInfo.ConversationId,
                    BotConversationId = conversationInfo.BotConversationId
                });
            }

            SendMessageAsync(tasks, conversationInfo, notificationMessage);
        }

        private void SendMessageAsync(List<Task<SendMessageResult>> tasks, ConversationInfo conversationInfo, NotificationMessage notificationMessage)
        {
            bool isEnableTelegramChannel = appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram);
            tasks.Add(!isEnableTelegramChannel ? Task.Run(() => new SendMessageResult
            {
                NotificationId = notificationMessage.NotificationId,
                ConversationId = conversationInfo.ConversationId,
                SentTime = DateTime.Now,
                ErrorCode = 1,
                ErrorMessage = Resources.Resources.DisableChannelErrorMessage,
                IsIgnoreRetry = true
            }) : Task.Run(() => SendMessageAsync(conversationInfo, notificationMessage.NotificationId, notificationMessage.Message)));
        }

        protected internal virtual async Task<SendMessageResult> SendMessageAsync(ConversationInfo conversationInfo, int notificationId, string messageContent)
        {
            if (conversationInfo.IsDuplicated)
            {
                return DuplicatedResult(notificationId, conversationInfo.ConversationId);
            }

            bool isDuplicated = messageLogService.IsInvalidSendMessageToBotService(conversationInfo.ConversationId, messageContent);

            if (isDuplicated)
            {
                return DuplicatedResult(notificationId, conversationInfo.ConversationId);
            }

            string guiid = Guid.NewGuid().ToString();
            try
            {
                _ = long.TryParse(conversationInfo.BotConversationId, out long botId);
                _ = long.TryParse(conversationInfo.ConversationId, out long chatId);
                await messageLogService.InserMessageLogIntoCache(conversationInfo.ConversationId, messageContent, guiid);
                SendMessageResult result = await SendTextMessageAsync(botId, chatId, notificationId, messageContent);
                if (result.ErrorCode != 0)
                {
                    await messageLogService.RemoveMessageLogOnCache(conversationInfo.ConversationId, guiid);
                }

                var thread = new Thread(async () => await AddMessageLogToDatabaseAsync(conversationInfo, messageContent, result));
                thread.Start();

                return result;
            }
            catch (Exception exception)
            {
                await messageLogService.RemoveMessageLogOnCache(conversationInfo.ConversationId, guiid);
                await WriteLogAsync(exception);
                return new SendMessageResult
                {
                    NotificationId = notificationId,
                    ConversationId = conversationInfo.ConversationId,
                    SentTime = DateTime.Now,
                    ErrorCode = 1,
                    ErrorMessage = exception.Message
                };
            }
        }

        private SendMessageResult DuplicatedResult(int notificationId, string conversationId)
            => new()
            {
                ConversationId = conversationId,
                SentTime = DateTime.UtcNow,
                ErrorCode = -1,
                ErrorMessage = Resources.Resources.DuplicatedErrorMessage,
                NotificationId = notificationId,
                IsIgnoreRetry = true
            };

        protected internal virtual async Task<SendMessageResult> SendTextMessageAsync(long botId, long chatId, int notificationId, string messageContent)
        {
            var result = new SendMessageResult
            {
                ConversationId = chatId.ToString(),
                NotificationId = notificationId
            };

            try
            {
                await botService.SendTextMessageAsync(botId, chatId, messageContent.ToMarkdownV2());
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
            }

            result.SentTime = DateTime.Now;
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