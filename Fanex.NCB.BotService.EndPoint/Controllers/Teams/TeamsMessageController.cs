namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using Core;
    using Core.Criteria;
    using Core.Enum;
    using Core.Services.Interfaces;
    using Fanex.NCB.BotService.Core.Helpers;
    using Logging.Extensions;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Shared.Models;
    using Shared.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TeamsMessageController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IConversationService _conversationService;
        private readonly IMessageLogService _messageLogService;
        private readonly ICommonLogService _commonLogService;
        private readonly IBotFrameworkService _botService;

        public TeamsMessageController(
            IOptions<AppSettings> appSettings,
            ICommonLogService commonLogService,
            IMessageLogService messageLogService,
            IConversationService conversationService,
            IBotFrameworkService botService)
        {
            this._appSettings = appSettings.Value;
            this._commonLogService = commonLogService;
            this._conversationService = conversationService;
            this._messageLogService = messageLogService;
            this._botService = botService;
        }

        [HttpPost]
        [Route("api/teams/notify")]
        public async Task<ObjectResult> Send([FromBody] ConversationInfo conversationInfo)
        {
            var errorMessage = string.Empty;
            try
            {
                if (_appSettings.EnableChannels.All(x => x != (byte)ConversationChannel.Teams))
                {
                    errorMessage = Resources.Resources.DisableChannelErrorMessage;
                }
                else
                {
                    await _botService.SendMessageAsync(conversationInfo);
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
        [Route("api/teams/notifymulti")]
        public async Task<ObjectResult> SendMulti([FromBody] IList<NotificationMessage> notificationMessages)
        {
            var results = new List<SendMessageResult>();
            var tasks = new List<Task<SendMessageResult>>();

            if (notificationMessages != null && notificationMessages.Any())
            {
                var notifications = _conversationService.GetDistinctNotificationConversation(notificationMessages);
                TransformMessageContentAvoidSpecialSyntax(notifications);

                foreach (var notification in notifications)
                {
                    foreach (var conversationInfo in notification.Recipients)
                    {
                        conversationInfo.NotificationId = notification.NotificationId;
                        conversationInfo.Message = notification.Message;
                        SendNotification(tasks, conversationInfo);
                    }
                }

                results = (await Task.WhenAll(tasks)).ToList();
            }

            return Ok(results);
        }

        [HttpPost]
        [Route("api/teams/sendIndirectMessage")]
        public async Task<ObjectResult> SendIndirectMessageAsync([FromBody] ConversationInfo conversationInfo)
        {
            var param = new Dictionary<string, object>
            {
                { "notificationId", conversationInfo.NotificationId },
                { "botId", conversationInfo.BotConversationId },
                { "chatId", conversationInfo.ConversationId },
                { "messageContent", conversationInfo.Message }
            };
            var logMessage = new LogMessage("Send indirect message")
                .AddSection("Params", param);
            _commonLogService.LogFileMessage(logMessage.ToString());

            var result = new SendMessageResult();
            try
            {
                await _botService.SendMessageAsync(conversationInfo);
            }
            catch (Exception exception)
            {
                result.ErrorCode = 1;
                result.ErrorMessage = exception.Message;

                logMessage = new LogMessage(exception.Message)
                    .AddSection("Params", param)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
                _commonLogService.LogFileMessage(logMessage.ToString());
            }

            return Ok(result);
        }

        private static void TransformMessageContentAvoidSpecialSyntax(IList<NotificationMessage> notificationMessages)
        {
            foreach (var msg in notificationMessages)
            {
                if (!string.IsNullOrWhiteSpace(msg.Message))
                {
                    string msgContent = msg.Message.FixTeamsTextFormat();
                    msg.Message = msgContent;
                }
            }
        }

        private void SendNotification(List<Task<SendMessageResult>> tasks, ConversationInfo conversationInfo)
        {
            bool isEnabledChannel = _appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Teams);
            tasks.Add(!isEnabledChannel ? Task.Run(() => new SendMessageResult
            {
                NotificationId = conversationInfo.NotificationId,
                ConversationId = conversationInfo.ConversationId,
                SentTime = DateTimeOffset.UtcNow.DateTime,
                ErrorCode = 1,
                ErrorMessage = Resources.Resources.DisableChannelErrorMessage,
                IsIgnoreRetry = true
            }) : Task.Run(() => SendNotification(conversationInfo)));
        }

        private async Task<SendMessageResult> SendNotification(ConversationInfo conversationInfo)
        {
            if (conversationInfo.IsDuplicated)
            {
                return DuplicatedResult(conversationInfo.NotificationId, conversationInfo.ConversationId);
            }

            bool isDuplicated = _messageLogService.IsInvalidSendMessageToBotService(conversationInfo.ConversationId, conversationInfo.Message);
            if (isDuplicated)
            {
                return DuplicatedResult(conversationInfo.NotificationId, conversationInfo.ConversationId);
            }

            var guid = Guid.NewGuid().ToString();
            try
            {
                await _messageLogService.InsertMessageLogIntoCache(conversationInfo.ConversationId, conversationInfo.Message, guid);

                var result = await _botService.SendMessageWithRetryAsync(conversationInfo);
                if (result.ErrorCode != 0)
                {
                    await _messageLogService.RemoveMessageLogOnCache(conversationInfo.ConversationId, guid);
                }

                var thread = new Thread(async () => await AddMessageLogOnDb(conversationInfo, result));
                thread.Start();

                return result;
            }
            catch (Exception exception)
            {
                await WriteLogAsync(exception);
                await _messageLogService.RemoveMessageLogOnCache(conversationInfo.ConversationId, guid);

                return new SendMessageResult
                {
                    NotificationId = conversationInfo.NotificationId,
                    ConversationId = conversationInfo.ConversationId,
                    SentTime = DateTimeOffset.UtcNow.DateTime,
                    ErrorCode = 1,
                    ErrorMessage = exception.Message
                };
            }
        }

        private static SendMessageResult DuplicatedResult(int notificationId, string conversationId)
            => new()
            {
                ConversationId = conversationId,
                SentTime = DateTimeOffset.UtcNow.DateTime,
                ErrorCode = -1,
                ErrorMessage = Resources.Resources.DuplicatedErrorMessage,
                NotificationId = notificationId,
                IsIgnoreRetry = true
            };

        private async Task AddMessageLogOnDb(ConversationInfo conversationInfo, SendMessageResult result)
        {
            try
            {
                var command = new AddMessageLogCommand
                {
                    FromConversationId = conversationInfo.BotConversationId,
                    ToConversationId = conversationInfo.ConversationId,
                    Content = conversationInfo.Message,
                    ReferenceId = result.NotificationId,
                    FileName = conversationInfo.Attachments != null && conversationInfo.Attachments.Count > 0 ?
                                        string.Join(",", conversationInfo.Attachments) : string.Empty,
                    IsSuccessfull = result.ErrorCode == 0,
                    ErrorMessage = result.ErrorMessage
                };

                await _conversationService.AddMessageLog(command);
            }
            catch (Exception ex)
            {
                await WriteLogAsync(ex);
            }
        }

        protected internal virtual async Task WriteLogAsync(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .ToString();
            await _commonLogService.SentryErrorAsync(logMessage, exception);
        }
    }
}