namespace Fanex.NCB.BotService.EndPoint.Controllers.Telegram;

using Logging.Extensions;
using Core;
using Core.Criteria;
using Core.Enum;
using Core.Services;
using Core.Services.Interfaces;
using Shared.Extensions;
using Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MessageController : Controller
{
    private readonly AppSettings appSettings;
        private readonly ICommonLogService logService;
    private readonly IConversationService conversationService;
    private readonly ITelegramBotService botService;
    private readonly IMessageLogService messageLogService;

    public MessageController(
        IOptions<AppSettings> appSettings,
            ICommonLogService logService,
        IConversationService conversationService,
        ITelegramBotService botService,
        IMessageLogService messageLogService)
    {
        this.appSettings = appSettings.Value;
            this.logService = logService;
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
                _ = long.TryParse(conversationInfo.BotConversationId, out var botId);
                _ = long.TryParse(conversationInfo.ConversationId, out var chatId);
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
            logService.LogFileMessage(JsonConvert.SerializeObject(notificationMessages));
        const short Milliseconds = 1000;
        var results = new List<SendMessageResult>();
        if (notificationMessages != null && notificationMessages.Any())
        {
            var notifications = conversationService.GetDistinctNotificationConversation(notificationMessages);
            var batches = DivideConversionByBatch(notifications);
            foreach (var batch in batches)
            {
                var tasks = new List<Task<SendMessageResult>>();
                foreach (var conversationInfo in batch) SendMessage(tasks, conversationInfo);

                results.AddRange((await Task.WhenAll(tasks)).ToList());
                await Task.Delay(Milliseconds);
            }
        }

        return Ok(results);
    }

    [HttpPost]
    [Route("api/telegram/sendIndirectMessage")]
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
            logService.LogFileMessage(logMessage.ToString());

        var result = new SendMessageResult();
        if (!appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram))
        {
            result.ErrorCode = 1;
            result.ErrorMessage = Resources.Resources.DisableChannelErrorMessage;
        }
        else
        {
            try
            {
                _ = long.TryParse(conversationInfo.BotConversationId, out var botId);
                _ = long.TryParse(conversationInfo.ConversationId, out var chatId);
                await botService.SendTextMessageAsync(botId, chatId, conversationInfo.Message.ToMarkdownV2());
            }
            catch (Exception exception)
            {
                result.ErrorCode = 1;
                result.ErrorMessage = exception.Message;

                logMessage = new LogMessage(exception.Message)
                    .AddSection("Params", param)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
                    logService.LogFileMessage(logMessage.ToString());
            }
        }

        return Ok(result);
    }

    private static IList<List<ConversationInfo>> DivideConversionByBatch(IEnumerable<NotificationMessage> notifications)
    {
        var floatConversations = new List<ConversationInfo>();
        var batches = new List<List<ConversationInfo>>();
        foreach (var notification in notifications)
        foreach (var conversationInfo in notification.Recipients)
        {
            conversationInfo.NotificationId = notification.NotificationId;
            conversationInfo.Message = notification.Message;
            floatConversations.Add(conversationInfo);

            if (!batches.Any()
                || floatConversations.Count(x => x.ConversationId == conversationInfo.ConversationId
                                                 && x.BotConversationId == conversationInfo.BotConversationId) >
                batches.Count)
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

        return batches;
    }

    private void SendMessage(List<Task<SendMessageResult>> tasks, ConversationInfo conversationInfo)
    {
        var isEnableTelegramChannel = appSettings.EnableChannels.Any(x => x == (byte)ConversationChannel.Telegram);
        tasks.Add(!isEnableTelegramChannel
            ? Task.Run(() => new SendMessageResult
            {
                NotificationId = conversationInfo.NotificationId,
                ConversationId = conversationInfo.ConversationId,
                SentTime = DateTimeOffset.UtcNow.DateTime,
                ErrorCode = 1,
                ErrorMessage = Resources.Resources.DisableChannelErrorMessage,
                IsIgnoreRetry = true
            })
            : Task.Run(() => SendMessageAsync(conversationInfo)));
    }

    protected internal virtual async Task<SendMessageResult> SendMessageAsync(ConversationInfo conversationInfo)
    {
            logService.LogFileMessage($"Failed image if existed: {JsonConvert.SerializeObject(conversationInfo.FailedImages)}");

        if (conversationInfo.IsDuplicated)
            return DuplicatedResult(conversationInfo.NotificationId, conversationInfo.ConversationId);

            if (conversationInfo.FailedImages is not null && conversationInfo.FailedImages.Count > 0)
            {
                logService.LogFileMessage($"Retry images {JsonConvert.SerializeObject(conversationInfo)}");
                return await botService.ResendImageOnlyAsync(conversationInfo);
            }


        if (isDuplicated) return DuplicatedResult(conversationInfo.NotificationId, conversationInfo.ConversationId);

        var guiid = Guid.NewGuid().ToString();
        try
        {
            await messageLogService.InsertMessageLogIntoCache(conversationInfo.ConversationId, conversationInfo.Message,
                SendMessageResult result = await botService.SendMessageWithImagesAsync(conversationInfo);
            var result = await botService.SendTextMessageAsync(conversationInfo);
            if (result.ErrorCode != 0)
                await messageLogService.RemoveMessageLogOnCache(conversationInfo.ConversationId, guiid);

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
                SentTime = DateTimeOffset.UtcNow.DateTime,
                ErrorCode = 1,
                ErrorMessage = exception.Message
            };
        }
    }

    private static SendMessageResult DuplicatedResult(int notificationId, string conversationId)
    {
        return new()
        {
            ConversationId = conversationId,
            SentTime = DateTimeOffset.UtcNow.DateTime,
            ErrorCode = -1,
            ErrorMessage = Resources.Resources.DuplicatedErrorMessage,
            NotificationId = notificationId,
            IsIgnoreRetry = true
        };
    }

    protected internal virtual async Task AddMessageLogToDatabaseAsync(ConversationInfo conversationInfo,
        SendMessageResult result)
    {
        try
        {
            var command = new AddMessageLogCommand
            {
                FromConversationId = conversationInfo.BotConversationId,
                ToConversationId = conversationInfo.ConversationId,
                Content = conversationInfo.Message,
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
            .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
        if (param != null) logMessage.AddSection("Params", param);

            await logService.SentryErrorAsync(logMessage.ToString(), exception);
    }
}