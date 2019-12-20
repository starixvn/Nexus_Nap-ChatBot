namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class SpecialDeleteTicketService : ISpecialDeleteTicketService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;
        private readonly ISpecialDeleteRepository specialDeleteRepository;

        public SpecialDeleteTicketService(IBaseService baseService,
            IChatbotService chatbotService,
            ISpecialDeleteRepository specialDeleteRepository)
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
            this.specialDeleteRepository = specialDeleteRepository;
        }

        public async Task PullTicketAndCreateNotification()
        {
            var systemSettings = await GetSystemSettings();

            if (systemSettings != null)
            {
                var messages = await GetMessageFromSpecialDelete(systemSettings);
                if (messages.Item1.Any() && messages.Item2.Any())
                {
                    var result = await ImportMessageToChatbotDB(messages.Item1, messages.Item2);

                    if (!result.Item1.IsSuccessful)
                    {
                        await chatbotService.NotifyToGroup(result.Item1.Message);
                    }
                    else
                    {
                        await chatbotService.SendMessageToMultitpleAsync(result.Item2);
                    }
                }
            }
        }

        private async Task<ValueTuple<CommandResult, IEnumerable<NotificationMessage>>> ImportMessageToChatbotDB(
            IEnumerable<Message> messages,
            IEnumerable<MessageDetail> messageDetails)
        {
            var criteria = new ImportMessageFromSpecialDeleteCriteria
            {
                MessageHeader = JsonConvert.SerializeObject(messages),
                MessageDetail = JsonConvert.SerializeObject(messageDetails)
            };

            var result = await specialDeleteRepository.ImportMessageAndGetNotification(criteria);

            return result;
        }

        private async Task<ValueTuple<IEnumerable<Message>, IEnumerable<MessageDetail>>> GetMessageFromSpecialDelete(SystemSetting systemSettings)
        {
            var criteria = new GetMessageFromSpecialDeleteCriteria
            {
                MessageID = systemSettings.MaxMessageID,
                BatchSize = systemSettings.BatchSize,
                MsgContentsTypeLst = systemSettings.MsgContentsTypeLst
            };

            var result = await baseService.FetchMultipleAsync<Message, MessageDetail>(criteria);

            if (result.Item1.Any())
            {
                foreach (var msg in result.Item1)
                {
                    var contentEn = !string.IsNullOrWhiteSpace(msg.Content_en) ? msg.Content_en.Trim() : string.Empty;
                    var contentCs = !string.IsNullOrWhiteSpace(msg.Content_cs) ? msg.Content_cs.Trim() : string.Empty;
                    msg.Content_en = contentEn;
                    msg.Content_cs = contentCs;
                }
            }

            return result;
        }

        private async Task<SystemSetting> GetSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }
    }
}