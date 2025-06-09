namespace Fanex.NCB.BotService.EndPoint.Controllers.Telegram;

using Core.Criteria;
using Core.Enum;
using Core.Services;
using Core.Services.Interfaces;
using Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fanex.NCB.BotService.Core.Models;
using Newtonsoft.Json;

public class UpdateController : Controller
{
    private readonly ICommonLogService logger;
    private readonly ITelegramBotService botService;
    private readonly IConversationService conversationService;
    private readonly IEnumerable<string> botUserNames;

    public UpdateController(
        ICommonLogService logger,
        ITelegramBotService botService,
        IConversationService conversationService,
        IEnumerable<TelegramBotConfigurations> botConfigurations)
    {
        this.logger = logger;
        this.botService = botService;
        this.conversationService = conversationService;
        this.botUserNames = botConfigurations.Select(bot => bot.UserName);
    }

#pragma warning disable S1541 // Methods and properties should not be too complex

    [HttpPost]
    [Route("api/telegram/webhook/{botId:long}/update")]
    public async Task<IActionResult> Post(long botId, [FromBody] Update update)
    {
        logger.SentryInfoAsync($"Received update for bot {botId}: {JsonConvert.SerializeObject(update)}");
        logger.SentryErrorAsync($"Received update for bot {botId}: {JsonConvert.SerializeObject(update)}");
        if (update == null || update.Type != UpdateType.Message) return Ok();

        var bot = await botService.GetBotInfoAsync(botId);
        if (bot == null)
        {
                await logger.SentryErrorAsync($"Telegram Bot {botId} is invalid.");
            return Ok($"Telegram Bot {botId} is invalid.");
        }

        var message = update.Message;
        await HandleBotEvents(bot, message);

        return Ok($"Telegram Bot {botId} is added successfully.");
    }

    private async Task HandleBotEvents(User bot, Message message)
    {
        await logger.SentryErrorAsync($"botUserNames: {string.Join(", ", botUserNames)}");
        await logger.SentryErrorAsync($"Bot: {JsonConvert.SerializeObject(bot)}");
        await logger.SentryErrorAsync($"Message: {JsonConvert.SerializeObject(message)}");

        switch (message.Type)
        {
            case MessageType.Text:
                await HandleBotCommandAsync(bot, message);
                break;

            case MessageType.ChatTitleChanged:
                await EditChatTitledAsync(message);
                break;

            case MessageType.GroupCreated:
                await AddBotToGroupAsync(message, bot);
                break;

            case MessageType.ChatMembersAdded:
                var bots = message.NewChatMembers.Where(x => x.IsBot && botUserNames.Contains(x.Username)).ToArray();
                for (var i = 0; i < bots.Length; i++)
                {
                    await AddBotToGroupAsync(message, bots[i]);
                }
                break;

            case MessageType.ChatMemberLeft:
                await RemoveBotToGroupAsync(message);
                break;

            case MessageType.MigratedToSupergroup:
                await MigratedToSupergroupAsync(message);
                break;

            default:
                // do nothing
                break;
        }
    }

#pragma warning restore S1541 // Methods and properties should not be too complex

    protected internal virtual async Task<CommandResult> EditChatTitledAsync(Message message)
    {
        var command = new UpdateGroupTopicCommand
        {
            ConversationId = message.Chat.Id.ToString(),
            GroupName = message.NewChatTitle,
            UpdatedBy = message.From.Id.ToString()
        };
        return await conversationService.UpdateGroupTopic(command);
    }

    protected internal virtual async Task<CommandResult> AddBotToGroupAsync(Message message, User bot)
    {
        var command = new AddBotToGroupCommand
        {
            BotConversationId = bot.Id.ToString(),
            BotName = bot.FirstName,
            ConversationId = message.Chat.Id.ToString(),
            GroupName = message.Chat.Title,
            ChannelId = (byte)ConversationChannel.Telegram,
            ServiceUrl = string.Empty,
            UpdatedBy = message.From.Id.ToString()
        };
        return await conversationService.AddBotToGroup(command);
    }

    protected internal virtual async Task<CommandResult> RemoveBotToGroupAsync(Message message)
    {
        if (message.LeftChatMember.IsBot)
        {
            var command = new RemoveBotOutGroupCommand
            {
                BotConversationId = message.LeftChatMember.Id.ToString(),
                ConversationId = message.Chat.Id.ToString(),
                UpdatedBy = message.From.Id.ToString()
            };
            return await conversationService.RemoveBotOutGroup(command);
        }

        return new CommandResult { Code = 0, Message = string.Empty };
    }

    protected internal virtual async Task<CommandResult> MigratedToSupergroupAsync(Message message)
    {
        var command = new MigratedToSupergroupCommand
        {
            FromConversationId = message.Chat.Id.ToString(),
            ToConversationId = message.MigrateToChatId.ToString(),
            UpdatedBy = message.From.Id.ToString()
        };
        return await conversationService.MigratedToSupergroupAsync(command);
    }

    protected internal virtual async Task HandleBotCommandAsync(User bot, Message message)
    {
        if (message.Entities is not null && message.Entities.Any())
        {
            var commands = message.Entities.Where(entity => entity.Type == MessageEntityType.BotCommand);
            await SendTextMessageAsync(bot, message, commands);
        }
    }

    protected internal virtual async Task SendTextMessageAsync(User bot, Message message,
        IEnumerable<MessageEntity> commands)
    {
        if (commands is not null && commands.Any())
        {
            var listText = commands.Select(entity => message.Text.Substring(entity.Offset, entity.Length)).ToList();
            foreach (var command in listText)
                if (command.Equals("/getgroupid", StringComparison.CurrentCultureIgnoreCase)
                    || command.Equals($"/getgroupid@{bot.Username}", StringComparison.CurrentCultureIgnoreCase))
                    await botService.SendTextMessageAsync(bot.Id, message.Chat.Id,
                        Regex.Replace(message.Chat.Id.ToString(), @"(.*?)(-|[\\][-])", "$1\\-"));
        }
    }
}