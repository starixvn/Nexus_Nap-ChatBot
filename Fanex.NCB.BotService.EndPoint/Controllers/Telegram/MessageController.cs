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
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageController : Controller
    {
        private const string DisableChannelErrorMessage = "Chatbot service has been stopped due to turning off the configuration by admin.";

        private readonly AppSettings appSettings;
        private readonly ILogger logger;
        private readonly IConversationService conversationService;
        private readonly ITelegramBotService botService;

        public MessageController(
            IOptions<AppSettings> appSettings,
            ILogger logger,
            IConversationService conversationService,
            ITelegramBotService botService)
        {
            this.appSettings = appSettings.Value;
            this.logger = logger;
            this.conversationService = conversationService;
            this.botService = botService;
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
                    await WriteLog(ex);
                }
            }

            return Ok(errorMessage);
        }

        [Route("api/telegram/sendMultiMessage")]
        [HttpPost]
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
                        tasks.Add(
                            Task.Run(() => SendMessageAsync(conversationInfo, notificationMessage.NotificationId, notificationMessage.Message))
                            );
                    }
                }
                results = (await Task.WhenAll(tasks)).ToList();
            }

            return Ok(results);
        }

        private async Task<SendMessageResult> SendMessageAsync(ConversationInfo conversationInfo, int notificationId, string text)
        {
            if (!appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram))
            {
                return new SendMessageResult
                {
                    NotificationId = notificationId,
                    ConversationId = conversationInfo.ConversationId,
                    SentTime = DateTime.Now,
                    ErrorCode = 1,
                    ErrorMessage = DisableChannelErrorMessage
                };
            }
            else
            {
                try
                {
                    _ = int.TryParse(conversationInfo.BotConversationId, out int botId);
                    _ = long.TryParse(conversationInfo.ConversationId, out long chatId);
                    SendMessageResult result = await SendMessageAsync(botId, chatId, notificationId, text);
                    var thread = new Thread(async () => await AddMessageLogAsync(conversationInfo, text, result));
                    thread.Start();

                    return result;
                }
                catch (Exception exception)
                {
                    await WriteLog(exception);
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
                await WriteLog(exception);

                result.ErrorCode = 1;
                result.ErrorMessage = exception.Message;
            }

            result.SentTime = DateTime.Now;
            return result;
        }

        private async Task AddMessageLogAsync(ConversationInfo conversationInfo, string text, SendMessageResult result)
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
                await WriteLog(ex);
            }
        }

        private async Task WriteLog(Exception exception)
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