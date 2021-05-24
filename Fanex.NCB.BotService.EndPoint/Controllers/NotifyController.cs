namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Enum;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotifyController : ControllerBase
    {
        private readonly IAdapterIntegration adapter;
        private readonly BotConfiguration botConfig;
        private readonly IConversationService conversationService;
        private readonly IMessageLogService messageLogService;
        private readonly CheckDuplicatedMessageLogSetting settings;
        private readonly ILogger logger;

        public NotifyController(IAdapterIntegration adapter,
            IMessageLogService messageLogService,
            BotConfiguration botConfig,
            IConversationService conversationService,
            CheckDuplicatedMessageLogSetting settings,
            ILogger logger)
        {
            this.adapter = adapter;
            this.botConfig = botConfig;
            this.conversationService = conversationService;
            this.messageLogService = messageLogService;
            this.settings = settings;
            this.logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/Test")]
        public async Task<ObjectResult> Test()
        {
            await logger.ErrorAsync("test sentry");

            return Ok("ok");
        }

        [HttpPost]
        [Route("api/notify")]
        public async Task<ObjectResult> Send(string botName, string conversationId, string message)
        {
            var appId = GetAppId(botName);
            var conversationReference = await GetConversationReference(conversationId, botName);
            var errorMessage = string.Empty;

            try
            {
                await SendMessage(appId, conversationReference, message);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                //await WriteLog(ex);
            }

            return Ok(errorMessage);
        }

        [AllowAnonymous]
        [Route("api/notifymulti")]
        [HttpPost]
        public async Task<ObjectResult> SendMulti([FromBody] IList<NotificationMessage> notificationMessages)
        {
            var results = new List<SendMessageResult>();
            var tasks = new List<Task<SendMessageResult>>();

            if (notificationMessages != null && notificationMessages.Any())
            {
                foreach (var notificationMessage in notificationMessages)
                {
                    foreach (var conversationInfo in notificationMessage.Recipients)
                    {
                        await AddTaskSendNotification(tasks, conversationInfo, notificationMessage);
                    }
                }
                results = (await Task.WhenAll(tasks)).ToList();
            }

            return Ok(results);
        }

        private async Task AddTaskSendNotification(List<Task<SendMessageResult>> tasks, ConversationInfo conversationInfo, NotificationMessage notificationMessage)
        {
            if (settings.IsEnabled)
            {
                bool IsExistedOnCacheOrExceptionList = await messageLogService.IsExistedOnCacheOrExceptionList(conversationInfo.ConversationId, notificationMessage.Message);

                tasks.Add(IsExistedOnCacheOrExceptionList ? Task.Run(() => new SendMessageResult
                {
                    ConversationId = conversationInfo.ConversationId,
                    SentTime = DateTime.UtcNow,
                    ErrorCode = -1,
                    ErrorMessage = $"The same message to this group has been send during {settings.TimeSpanInMinutes} minutes",
                    NotificationId = notificationMessage.NotificationId,
                    IsIgnoreRetry = true
                }) : Task.Run(() => SendNotification(conversationInfo, notificationMessage.NotificationId, notificationMessage.Message))
);
            }
            else
            {
                tasks.Add(
                    Task.Run(() => SendNotification(conversationInfo, notificationMessage.NotificationId, notificationMessage.Message))
                    );
            }
        }

        private async Task<SendMessageResult> SendNotification(ConversationInfo conversationInfo, int notifictaionId, string message)
        {
            try
            {
                var conversationReference = GetConversationReference(conversationInfo);
                var appId = GetAppId(conversationInfo.BotName);
                var result = await SendMessageNotification(appId, notifictaionId, conversationReference, message);
                var thread = new Thread(async () => await AddMessageLog(conversationReference, message, result));
                thread.Start();

                if (settings.IsEnabled)
                {
                    InsertMessageInCache(result, message);
                }

                return result;
            }
            catch (Exception exception)
            {
                await WriteLog(exception);
                return new SendMessageResult
                {
                    NotificationId = notifictaionId,
                    ConversationId = conversationInfo.ConversationId,
                    SentTime = DateTime.UtcNow,
                    ErrorCode = 1,
                    ErrorMessage = exception.Message
                };
            }
        }

        private void InsertMessageInCache(SendMessageResult sendMessageResult, string messageContent)
        {
            var logMessage = new ConversationMessageLog
            {
                ConversationId = sendMessageResult.ConversationId,
                Message = messageContent,
                Status = sendMessageResult.ErrorCode == 0 ? SentMessageStatus.Success : SentMessageStatus.Fail,
                SentDate = DateTime.UtcNow
            };
            var threadInsertInRedis = new Thread(async () => await messageLogService.InserMessageLogInRedisCache(logMessage));
            threadInsertInRedis.Start();
        }

        private async Task<SendMessageResult> SendMessageNotification(string appId, int notificationId, ConversationReference conversationReference, string message)
        {
            var result = new SendMessageResult
            {
                ConversationId = conversationReference.Conversation.Id,
                NotificationId = notificationId
            };

            try
            {
                await SendMessage(appId, conversationReference, message);
            }
            catch (Exception exception)
            {
                result.ErrorCode = 1;
                result.ErrorMessage = exception.Message;

                await WriteLog(exception);
            }

            result.SentTime = DateTime.UtcNow;
            return result;
        }

        private async Task SendMessage(string appId, ConversationReference conversationReference, string message)
        {
            await adapter.ContinueConversationAsync(appId, conversationReference,
                    async (context, token) =>
                    {
                        MicrosoftAppCredentials.TrustServiceUrl(conversationReference.ServiceUrl);
                        await context.SendActivityAsync(message);
                    },
                    new CancellationToken());
        }

        private async Task AddMessageLog(ConversationReference conversationReference, string messageContent, SendMessageResult result)
        {
            try
            {
                var command = new AddMessageLogCommand
                {
                    FromConversationId = conversationReference.Bot.Id,
                    ToConversationId = conversationReference.Conversation.Id,
                    Content = messageContent,
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

        private async Task<ConversationReference> GetConversationReference(string conversationId, string botName)
        {
            var conversationGroup = await conversationService.GetGroupConversation(conversationId, botName);
            var coversation = new ConversationReference
            {
                Bot = new ChannelAccount { Id = conversationGroup.BotConversationId, Name = conversationGroup.BotName, Role = "bot" },
                Conversation = new ConversationAccount { Id = conversationId },
                ServiceUrl = conversationGroup.ServiceUrl,
                ChannelId = conversationGroup.ChannelId
            };

            return coversation;
        }

        private static ConversationReference GetConversationReference(ConversationInfo conversationInfo)
        {
            var coversation = new ConversationReference
            {
                Bot = new ChannelAccount { Id = conversationInfo.BotConversationId, Name = conversationInfo.BotName, Role = "bot" },
                Conversation = new ConversationAccount { Id = conversationInfo.ConversationId },
                ServiceUrl = conversationInfo.ServiceUrl,
                ChannelId = conversationInfo.ChannelId
            };

            return coversation;
        }

        private string GetAppId(string botName)
        {
            var endpoint = (EndpointService)botConfig.Services.FirstOrDefault(t => t.Name.Equals(botName, StringComparison.InvariantCultureIgnoreCase));
            string appId;

            if (endpoint != null && !string.IsNullOrEmpty(endpoint.AppId))
            {
                appId = endpoint.AppId;
            }
            else
            {
                appId = Guid.NewGuid().ToString();
                logger.Error($"BotName : { botName } is not supported by system.");
            }

            return appId;
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