namespace Fanex.NCB.BotService.EndPoint.Controllers.Telegram
{
    using System.Linq;
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
        public async Task<IActionResult> Post(long botId, [FromBody] Update update)
        {
            if (update == null || update.Type != UpdateType.Message)
            {
                return Ok();
            }

            User bot = await botService.GetBotInfoAsync(botId);
            if(bot == null)
            {
                await logger.ErrorAsync($"Telegram Bot {botId} is invalid.");
                return Ok($"Telegram Bot {botId} is invalid.");
            }

            Message message = update.Message;
            switch (message.Type)
            {
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
                    if (message.LeftChatMember.IsBot)
                    {
                        await RemoveBotToGroupAsync(message);
                    }
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
            var command = new RemoveBotOutGroupCommand
            {
                BotConversationId = message.LeftChatMember.Id.ToString(),
                ConversationId = message.Chat.Id.ToString(),
                UpdatedBy = message.From.Id.ToString()
            };
            return await conversationService.RemoveBotOutGroup(command);
        }
    }
}