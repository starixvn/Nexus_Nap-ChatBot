namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Model.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class ChatGroupMappingService : IChatGroupMappingService
    {
        private readonly IBaseService baseService;

        public ChatGroupMappingService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMapping model)
        {
            var critera = new GetChapGroupMappingCriteria
            {
                BotIds = model.BotIDs,
                RecipientName = model.RecipientName,
                RecipientTypeIDs = model.RecipientTypeIDs,
                LicenseeStatusIds = model.LicenseeStatusIds,
                MappingStatusIds = model.MappingStatusIds,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
            };

            var result = await baseService.FetchMultipleAsync<BotConversation, BotConversationMember, ConversationRecipient>(critera).ConfigureAwait(false);

            return (MappingData(result.Item1, result.Item2, result.Item3), critera.TotalItems);
        }

        public async Task<CommandResult> EditAsync(EditChatGroupMapping model)
        {
            var command = new EditChatGroupMappingCommand
            {
                GroupID = model.GroupID,
                ChatGroupTypeId = model.ChatGroupTypeId,
                RecipientTypeID = model.RecipientTypeID,
                RecipientIDs = model.RecipientIDs,
                CreatorID = model.CreatorID,
                Remark = model.Remark
            };

            return await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);
        }

        private IEnumerable<ChatGroupMappingResult> MappingData(IEnumerable<BotConversation> conversations, IEnumerable<BotConversationMember> members, IEnumerable<ConversationRecipient> conversationRecipients)
        {
            var chatGroupMappings = new List<ChatGroupMappingResult>();
            foreach (var conversation in conversations)
            {
                var member = members.FirstOrDefault(m => m.ConversationID == conversation.ConversationID);
                var recipients = conversationRecipients.Where(r => r.GroupID == conversation.GroupID);
                var chatGroupMapping = new ChatGroupMappingResult
                {
                    BotName = member?.BotName,
                    GroupID = conversation.GroupID,
                    ConversationID = conversation.ConversationID,
                    GroupName = conversation.GroupName,
                    ChatGroupTypeID = conversation.ChatGroupTypeID,
                    ChatGroupTypeName = conversation.ChatGroupTypeName,
                    RecipientTypeID = conversation.RecipientTypeId,
                    RecipientTypeName = conversation.RecipientTypeName,
                    RecipientIDs = string.Join(",", recipients.Select(x => x.RecipientID)),
                    RecipientNames = string.Join(", ", recipients.Select(x => x.RecipientName)),
                    RecipientStatusNames = string.Join(", ", recipients.Where(x => !string.IsNullOrEmpty(x.RecipientStatusName))
                                                                        .Select(r => r.RecipientStatusName)),
                    MappingStatusID = (int)conversation.MappingStatus,
                    MappingStatusName = EnumHelper.GetDescription(conversation.MappingStatus),
                    Remark = conversation.Remark
                };
                chatGroupMappings.Add(chatGroupMapping);
            }

            return chatGroupMappings;
        }
    }
}