namespace Fanex.NCB.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Core.Services.Interfaces;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Model.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;

    public class ChatGroupMappingService : IChatGroupMappingService
    {
        private readonly IChatGroupMappingRepository chatGroupMappingRepository;
        private readonly IBaseService baseService;

        public ChatGroupMappingService(IChatGroupMappingRepository chatGroupMappingRepository, IBaseService baseService)
        {
            this.baseService = baseService;
            this.chatGroupMappingRepository = chatGroupMappingRepository;
        }

        public async Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria)
        {
            var tuple = await baseService.FetchMultipleAsync<BotConversation, BotConversationMember, ConversationRecipient>(criteria).ConfigureAwait(false);

            IEnumerable<ChatGroupMappingResult> groupMappings = MappingData(tuple.Item1, tuple.Item2, tuple.Item3);
            return new ValueTuple<IEnumerable<ChatGroupMappingResult>, long>(groupMappings, criteria.TotalItems);
        }

        private IEnumerable<ChatGroupMappingResult> MappingData(IEnumerable<BotConversation> conversations, IEnumerable<BotConversationMember> members, IEnumerable<ConversationRecipient> conversationRecipients)
        {
            var chatGroupMappings = new List<ChatGroupMappingResult>();
            foreach (var conversation in conversations)
            {
                var member = members.FirstOrDefault(m => m.ConversationID == conversation.ConversationID);
                var recipients = conversationRecipients.Where(r => r.GroupID == conversation.GroupID);
                MappingStatus mappingStatus = CalculateMappingStatusId(conversation.IsMapped, member);
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

        public async Task<CommandResult> Update(EditChatGroupMapping model)
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
    }
}