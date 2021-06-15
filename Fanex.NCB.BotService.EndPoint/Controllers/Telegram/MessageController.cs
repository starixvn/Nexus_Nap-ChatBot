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
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageController : Controller
    {
        private const string DisableChannelErrorMessage = "Chatbot service has been stopped due to turning off the configuration by admin.";
        private const string DuplicatedErrorMessage = "The content of the message sent to this group is duplicated for a period of time.";

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
        public async Task<ObjectResult> SendMessage(int botId, long chatId, string text)
        {
            var errorMessage = string.Empty;
            if (!appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram))
            {
                errorMessage = DisableChannelErrorMessage;
            }
            else
            {
                try
                {
                    await botService.SendTextMessageAsync(botId, chatId, text);
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    await WriteLogAsync(ex);
                }
            }

            return Ok(errorMessage);
        }

        [HttpPost]
        [Route("api/telegram/sendMultiMessage")]
        public async Task<ObjectResult> SendMultiMessage([FromBody] IList<NotificationMessage> notificationMessages)
        {
            var results = new List<SendMessageResult>();
            var tasks = new List<Task<SendMessageResult>>();
            if (notificationMessages != null && notificationMessages.Any())
            {
                foreach (NotificationMessage notificationMessage in notificationMessages)
                {
                    foreach (ConversationInfo conversationInfo in notificationMessage.Recipients)
                    {
                        tasks.Add(Task.Run(() => SendMessageAsync(conversationInfo, notificationMessage)));
                    }
                }
                results = (await Task.WhenAll(tasks)).ToList();
            }

            return Ok(results);
        }

        private async Task<SendMessageResult> SendMessageAsync(ConversationInfo conversationInfo, NotificationMessage notificationMessage)
        {
            if (!appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram))
            {
                return new SendMessageResult
                {
                    NotificationId = notificationMessage.NotificationId,
                    ConversationId = conversationInfo.ConversationId,
                    SentTime = DateTime.Now,
                    ErrorCode = 1,
                    ErrorMessage = DisableChannelErrorMessage,
                    IsIgnoreRetry = true
                };
            }
            else
            {
                try
                {
                    bool isDuplicated = await messageLogService.IsInvalidSendMessageToBotService(conversationInfo.ConversationId, notificationMessage.Message);
                    if (isDuplicated)
                    {
                        return new SendMessageResult
                        {
                            ConversationId = conversationInfo.ConversationId,
                            SentTime = DateTime.UtcNow,
                            ErrorCode = -1,
                            ErrorMessage = DuplicatedErrorMessage,
                            NotificationId = notificationMessage.NotificationId,
                            IsIgnoreRetry = true
                        };
                    }
                    else
                    {
                        _ = int.TryParse(conversationInfo.BotConversationId, out int botId);
                        _ = long.TryParse(conversationInfo.ConversationId, out long chatId);
                        SendMessageResult result = await SendMessageAsync(botId, chatId, notificationMessage.NotificationId, notificationMessage.Message);
                        var thread = new Thread(async () => await AddMessageLogAsync(conversationInfo, notificationMessage.Message, result));
                        thread.Start();

                        return result;
                    }
                }
                catch (Exception exception)
                {
                    await WriteLogAsync(exception);
                    return new SendMessageResult
                    {
                        NotificationId = notificationMessage.NotificationId,
                        ConversationId = conversationInfo.ConversationId,
                        SentTime = DateTime.Now,
                        ErrorCode = 1,
                        ErrorMessage = exception.Message
                    };
                }
            }
        }

        private async Task<SendMessageResult> SendMessageAsync(int botId, long chatId, int notificationId, string text)
        {
            var result = new SendMessageResult
            {
                ConversationId = chatId.ToString(),
                NotificationId = notificationId
            };

            try
            {
                await botService.SendTextMessageAsync(botId, chatId, text);
            }
            catch (Exception exception)
            {
                await WriteLogAsync(exception);

                result.ErrorCode = 1;
                result.ErrorMessage = exception.Message;
            }

            result.SentTime = DateTime.Now;
            return result;
        }

        private async Task AddMessageLogAsync(ConversationInfo conversationInfo, string text, SendMessageResult result)
        {
            await AddMessageLogToDatabaseAsync(conversationInfo, text, result);
            await AddMessageLogToCacheAsync(result, text);
        }

        private async Task AddMessageLogToDatabaseAsync(ConversationInfo conversationInfo, string text, SendMessageResult result)
        {
            try
            {
                var command = new AddMessageLogCommand
                {
                    FromConversationId = conversationInfo.BotConversationId,
                    ToConversationId = conversationInfo.ConversationId,
                    Content = text,
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

        private async Task AddMessageLogToCacheAsync(SendMessageResult sendMessageResult, string text)
        {
            var logMessage = new ConversationMessageLog
            {
                ConversationId = sendMessageResult.ConversationId,
                Message = text,
                Status = sendMessageResult.ErrorCode == 0 ? SentMessageStatus.Success : SentMessageStatus.Fail,
                SentDate = DateTime.UtcNow
            };

            await messageLogService.InserMessageLogIntoCache(logMessage);
        }

        private async Task WriteLogAsync(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                        .ToString();
            await logger.ErrorAsync(logMessage);
        }
    }
}