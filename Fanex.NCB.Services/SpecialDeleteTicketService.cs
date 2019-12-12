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

            if (systemSettings != null && systemSettings.JobStatus != 0)
            {
                IEnumerable<SpecialDeleteMessage> messages = await PullMessageFromSpecialDelete(systemSettings);

                var importResult = await ImportMessageToChatbotDB(messages);

                if (!importResult.IsSuccessful)
                {
                    await chatbotService.NotifySponDemoChatGroup(importResult.Message);
                }
            }
        }

        private async Task<CommandResult> ImportMessageToChatbotDB(IEnumerable<SpecialDeleteMessage> messages)
        {
            ValueTuple<string, string> tuple = ConvertMessageToJson(messages);
            var command = new ImportMessageFromSpecialDeleteCommand
            {
                MessageHeader = tuple.Item1,
                MessageDetail = tuple.Item2
            };

            return await baseService.ExecuteNonQueryAsync(command);
        }

        private ValueTuple<string, string> ConvertMessageToJson(IEnumerable<SpecialDeleteMessage> messages)
        {
            var messageHeaders = new List<MessageHeader>(
                messages.GroupBy(m => m.MessageId)
                .Select(g => new MessageHeader
                {
                    MessageID = g.Key,
                    ContentEN = messages.FirstOrDefault(m => m.MessageId == g.Key)?.ContentEN,
                    ContentCS = messages.FirstOrDefault(m => m.MessageId == g.Key)?.ContentCS
                }));

            var messageDetails = new List<MessageDetail>(
                    messages.Select(m => new MessageDetail
                    {
                        MessageID = m.MessageId,
                        Site = m.Site
                    })
                );

            return new ValueTuple<string, string>(JsonConvert.SerializeObject(messageHeaders), JsonConvert.SerializeObject(messageDetails));
        }

        private async Task<IEnumerable<SpecialDeleteMessage>> PullMessageFromSpecialDelete(SystemSetting systemSettings)
        {
            var criteria = new GetMessageFromSpecialDeleteCriteria
            {
                MessageID = systemSettings.PreviousMaxMessageId,
                BatchSize = systemSettings.BatchSize
            };

            return await baseService.GetDataAsync<SpecialDeleteMessage>(criteria);
        }

        private async Task<SystemSetting> GetSystemSettings()
        {
            var criteria = new GetSystemSettingCriteria();

            return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
        }
    }
}