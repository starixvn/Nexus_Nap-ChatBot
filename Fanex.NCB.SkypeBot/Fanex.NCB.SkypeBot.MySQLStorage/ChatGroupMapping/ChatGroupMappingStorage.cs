using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.MySQLStorage.ChatGroupMapping
{
    public class ChatGroupMappingStorage : IChatGroupMappingStorage
    {
        private readonly IBaseStorage _baseStorage;

        public ChatGroupMappingStorage(IBaseStorage baseStorage)
        {
            _baseStorage = baseStorage;
        }

        public async Task<List<BotName>> GetBotNameAsync()
        {
            return await _baseStorage.QueryAsync<BotName>("NCB_BotName_Sel").ConfigureAwait(false);
        }

        public async Task<List<MappingStatus>> GetMappingStatusAsync()
        {
            return await _baseStorage.QueryAsync<MappingStatus>("NCB_MappingStatus_Sel").ConfigureAwait(false);
        }
    }
}
