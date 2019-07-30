namespace Fanex.NCB.Services
{
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Core.Services.Interfaces;
    using Fanex.NCB.Criteria.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ChatGroupMappingService : IChatGroupMappingService
    {
        private readonly IChatGroupMappingRepository chatGroupMappingRepository;
        private readonly IBaseService baseService;

        public ChatGroupMappingService(IChatGroupMappingRepository chatGroupMappingRepository, IBaseService baseService)
        {
            this.baseService = baseService;
            this.chatGroupMappingRepository = chatGroupMappingRepository;
        }

        public async Task<(IEnumerable<ChatGroupMappingResult>, long)> GetChatGroupMappingListAsync(GetChapGroupMapping model)
        {
            return await chatGroupMappingRepository.GetChatGroupMappingListAsync(model).ConfigureAwait(false);
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