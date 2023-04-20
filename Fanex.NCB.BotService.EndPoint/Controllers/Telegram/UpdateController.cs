namespace Fanex.NCB.BotService.EndPoint.Controllers.Telegram
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Enum;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using global::Telegram.Bot.Types;
    using global::Telegram.Bot.Types.Enums;
    using Microsoft.AspNetCore.Mvc;

    public class UpdateController : Controller
    {
        private readonly ILogger logger;
        private readonly ITelegramBotService botService;
        private readonly IConversationService conversationService;

        public UpdateController(
            ILogger logger,
            ITelegramBotService botService,
            IConversationService conversationService)
        {
            this.logger = logger;
            this.botService = botService;
            this.conversationService = conversationService;
        }

        [HttpPost]
        [Route("api/telegram/webhook/{botId:long}/update")]
#pragma warning restore S1541 // Methods and properties should not be too complex
        public async Task<IActionResult> Post(long botId, [FromBody] Update update)
        {
            if (update == null || update.Type != UpdateType.Message)
            {
                return Ok();
            }

            User bot = await botService.GetBotInfoAsync(botId);
            if (bot == null)
            {
                await logger.ErrorAsync($"Telegram Bot {botId} is invalid.");
                return Ok($"Telegram Bot {botId} is invalid.");
            }

            Message message = update.Message;
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
                    User[] bots = message.NewChatMembers.Where(x => x.IsBot).ToArray();
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

            return Ok();
        }

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
                if (commands is not null && commands.Any())
                {
                    var listText = commands.Select(entity => message.Text.Substring(entity.Offset, entity.Length)).ToList();
                    foreach (var command in listText)
                    {
                        if (command.Equals("/getgroupid", StringComparison.CurrentCultureIgnoreCase)
                            || command.Equals($"/getgroupid@{bot.Username}", StringComparison.CurrentCultureIgnoreCase))
                        {
                            await botService.SendTextMessageAsync(bot.Id, message.Chat.Id, Regex.Replace(message.Chat.Id.ToString(), @"(.*?)(-|[\\][-])", "$1\\-"));
                        }
                    }
                }
            }
        }
    }
}