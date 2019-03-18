using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<ResultModel> EditChatGroupMappingAsync(EditChatGroupMappingCriteria item)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("groupID", value: item.GroupID, dbType: DbType.Int32);
            param.Add("conversationID", value: item.ConversationID, dbType: DbType.String);
            param.Add("recipientIDs", value: item.RecipientIDs, dbType: DbType.String);
            param.Add("recipientTypeID", value: item.RecipientTypeID, dbType: DbType.Int32);
            param.Add("creatorID", value: item.CreatorID, dbType: DbType.Int32);
            param.Add("remark", value: item.Remark, dbType: DbType.String);

            return await _baseStorage.ExecuteAsync("NCB_ChatGroupMapping_Upd", param).ConfigureAwait(false);
        }

        public async Task<List<BotName>> GetBotNameAsync()
        {
            return await _baseStorage.QueryAsync<BotName>("NCB_BotName_Sel").ConfigureAwait(false);
        }

        public async Task<Tuple<int, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria)
        {
            return await _baseStorage.QueryMultipleAsync<int, BotConversation, BotConversationMember, ConversationRecipient>("NCB_ChatGroupMapping_Sel", criteria).ConfigureAwait(false);
        }

        public async Task<List<MappingStatus>> GetMappingStatusAsync()
        {
            return await _baseStorage.QueryAsync<MappingStatus>("NCB_MappingStatus_Sel").ConfigureAwait(false);
        }
    }
}
