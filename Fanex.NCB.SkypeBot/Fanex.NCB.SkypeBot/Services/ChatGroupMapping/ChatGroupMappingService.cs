using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.SkypeBot.BaseStorage;
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
    }
}
