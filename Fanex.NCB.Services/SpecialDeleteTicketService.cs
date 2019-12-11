namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.SpecialDeleteTicket;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class SpecialDeleteTicketService : ISpecialDeleteTicketService
    {
        private readonly IBaseService baseService;
        private readonly IChatbotService chatbotService;

        public SpecialDeleteTicketService(IBaseService baseService,
            IChatbotService chatbotService)
        {
            this.baseService = baseService;
            this.chatbotService = chatbotService;
        }

        public async Task PullTicketAndCreateNotification()
        {
            var systemSettings = await GetSystemSettings();

            if (systemSettings != null)
            {
                var messages = await GetMessageFromSpecialDelete(systemSettings);

                if (messages.Item1.Any() && messages.Item2.Any())
                {
                    var importResult = await ImportMessageToChatbotDB(messages.Item1, messages.Item2);

                    if (!importResult.IsSuccessful)
                    {
                        await chatbotService.NotifySponDemoChatGroup(importResult.Message);
                    }
                }
            }
        }

        private async Task<CommandResult> ImportMessageToChatbotDB(IEnumerable<Message> messages,
            IEnumerable<MessageDetail> messageDetails)
        {
            WriteLogToFile.Writelog(messages);
            WriteLogToFile.Writelog(messageDetails);

            var command = new ImportMessageFromSpecialDeleteCommand
            {
                MessageHeader = JsonConvert.SerializeObject(messages),
                MessageDetail = JsonConvert.SerializeObject(messageDetails)
            };

            var result = await baseService.ExecuteNonQueryAsync(command);
            WriteLogToFile.Writelog(result);

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

            return await baseService.FetchMultipleAsync<Message, MessageDetail>(criteria);
        }

        private async Task<SystemSetting> GetSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }
    }
}