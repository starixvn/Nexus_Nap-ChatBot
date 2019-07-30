namespace Fanex.NCB.Repository
{
    using Fanex.Data.Repository;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Core.Repositories;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Model.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ChatGroupMappingRepository : BaseRepository, IChatGroupMappingRepository
    {
        private readonly IDynamicRepository dynamicRepository;

        public ChatGroupMappingRepository(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMapping model)
        {
            var critera = new GetChapGroupMappingCriteria
            {
                BotIDs = model.BotIDs,
                RecipientName = model.RecipientName,
                RecipientTypeIDs = model.RecipientTypeIDs,
                LicenseeStatusIds = model.LicenseeStatusIds,
                MappingStatusIds = model.MappingStatusIds,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize,
            };

            var result = await dynamicRepository.FetchMultipleAsync<BotConversation, BotConversationMember, ConversationRecipient>(critera).ConfigureAwait(false);

            return (MappingData(result.Item1, result.Item2, result.Item3), critera.TotalItems);
        }

        public async Task<CommandResult> Edit(EditChatGroupMapping model)
        {
            return await ExecuteNonQueryAsync(new EditChatGroupMappingCommand()).ConfigureAwait(false);
        }

        private IEnumerable<ChatGroupMappingResult> MappingData(IEnumerable<BotConversation> conversations, IEnumerable<BotConversationMember> members, IEnumerable<ConversationRecipient> conversationRecipients)
        {
            var chatGroupMappings = new List<ChatGroupMappingResult>();
            foreach (var conversation in conversations)
            {
                var member = members.FirstOrDefault(m => m.ConversationID == conversation.ConversationID);
                var recipients = conversationRecipients.Where(r => r.GroupID == conversation.GroupID);
                var mappingStatus = CalculateMappingStatusId(conversation.IsMapped, member);
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
                    RecipientIDs = string.Join(", ", recipients.Select(x => x.RecipientID)),
                    RecipientNames = string.Join(", ", recipients.Select(x => x.RecipientName)),
                    RecipientStatusNames = string.Join(", ", recipients.Where(x => !string.IsNullOrEmpty(x.RecipientStatusName))
                                                                        .Select(r => r.RecipientStatusName)),
                    MappingStatusID = (int)mappingStatus,
                    MappingStatusName = EnumHelper.GetDescription(mappingStatus),
                    Remark = conversation.Remark
                };
                chatGroupMappings.Add(chatGroupMapping);
            }

            return chatGroupMappings;
        }

        private MappingStatus CalculateMappingStatusId(bool isMapped, BotConversationMember member)
        {
            var isBotActive = member != null && member.IsActive;
            if (!isBotActive)
            {
                return MappingStatus.BotRemoved;
            }

            return isMapped ? MappingStatus.Mapped : MappingStatus.NotMapped;
        }
    }
}