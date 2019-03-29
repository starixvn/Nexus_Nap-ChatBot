using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.Common.DTO;
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
            return await _chatGroupMappingStorage.GetBotNameAsync();
        }

        public async Task<ChatGroupMappingResultModel> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria)
        {
            var data = await _chatGroupMappingStorage.GetChatGroupMappingListAsync(criteria);
            var mappingStatuses = GetMappingStatus();

            var result = new ChatGroupMappingResultModel
            {
                Count = data.Item1,
                Data = data.Item2.Select(g =>
                {
                    var botConversationMember = data.Item3.FirstOrDefault(i => i.ConversationID == g.ConversationID);
                    var conversationRecipients = data.Item4.Where(i => i.GroupID == g.GroupID).ToList();
                    var mappingStatusID = CalculateMappingStatus(g.IsMapped, botConversationMember != null && botConversationMember.IsActive);
                    return new ChatGroupMappingResult
                    {
                        BotName = botConversationMember?.BotName,
                        GroupID = g.GroupID,
                        ConversationID = g.ConversationID,
                        GroupName = g.GroupName,
                        RecipientTypeID = g.RecipientTypeId,
                        RecipientTypeName = g.RecipientTypeName,
                        RecipientIDs = String.Join(",", conversationRecipients.Select(x => x.RecipientID).ToList()),
                        RecipientNames = String.Join(", ", conversationRecipients.Select(x => x.RecipientName).ToList()),
                        RecipientStatusNames = String.Join(", ", conversationRecipients.Where(x => x.RecipientStatusName != string.Empty).Select(x => x.RecipientStatusName).ToList()),
                        MappingStatusID = mappingStatusID,
                        MappingStatusName = mappingStatuses.First(x => x.Id == mappingStatusID).Name,
                        Remark = g.Remark
                    };
                }).ToList()
            };

            return result;
        }

        public List<MappingStatus> GetMappingStatus()
        {
            var data = new List<MappingStatus> {
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.NotMapped, Name = "Not Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.Mapped, Name = "Mapped" },
                new MappingStatus { Id = (int)Common.Constant.MappingStatus.BotRemoved, Name = "Bot Removed" }
            };

            return data;
        }

        public Task<ResultModel> UpdateChatGroupMappingAsync(EditChatGroupMappingCriteria criteria)
        {
            return _chatGroupMappingStorage.EditChatGroupMappingAsync(criteria);
        }

        private static int CalculateMappingStatus(bool isConversationMapped, bool isBotActive)
        {
            if (!isBotActive)
            {
                return (int)Common.Constant.MappingStatus.BotRemoved;
            }

            return isConversationMapped ? (int)Common.Constant.MappingStatus.Mapped : (int)Common.Constant.MappingStatus.NotMapped;
        }
    }
}
