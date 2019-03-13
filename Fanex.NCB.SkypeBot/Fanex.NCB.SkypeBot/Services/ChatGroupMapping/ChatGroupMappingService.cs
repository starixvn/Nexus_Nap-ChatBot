using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;

namespace Fanex.NCB.SkypeBot.Services.ChatGroupMapping
{
    public class ChatGroupMappingService : IChatGroupMappingService
    {
        private readonly IChatGroupMappingStorage _chatGroupMappingStorage;
        public ChatGroupMappingService(IChatGroupMappingStorage chatGroupMappingStorage)
        {
            _chatGroupMappingStorage = chatGroupMappingStorage;
        }

        public async Task<List<BotName>> GetBotNameAsync()
        {
            return await _chatGroupMappingStorage.GetBotNameAsync().ConfigureAwait(false);            
        }

        public async Task<ChatGroupMappingResultModel> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria)
        {
            var data = await _chatGroupMappingStorage.GetChatGroupMappingListAsync(criteria).ConfigureAwait(false);
            var result = new ChatGroupMappingResultModel
            {
                Count = data.Item1,
                Data = data.Item2.GroupBy(i => i.ConversationID).Select(g =>
                {
                    return new ChatGroupMappingItem
                    {
                        BotName = g.First().BotName,
                        ConversationID = g.Key,
                        GroupName = g.First().GroupName,
                        RecipientTypeID = g.First().RecipientTypeID,
                        RecipientTypeName = g.First().RecipientTypeName,
                        RecipientID = g.First().RecipientID,
                        RecipientName = String.Join(", ", g.Select(x => x.RecipientName).ToList()), 
                        MappingStatusID = g.First().MappingStatusID,
                        MappingStatusName = g.First().MappingStatusName,
                        Remark = g.First().Remark
                    };
                }).ToList()
            };

            return result;
        }

        public async Task<List<MappingStatus>> GetMappingStatusAsync()
        {
            return await _chatGroupMappingStorage.GetMappingStatusAsync().ConfigureAwait(false);
        }
    }
}
