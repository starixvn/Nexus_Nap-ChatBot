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

        public Task<ResultModel> EditChatGroupMappingAsync(EditChatGroupMappingCriteria item)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("in_GroupID", value: item.GroupID, dbType: DbType.Int32);
            param.Add("in_RecipientIDs", value: item.RecipientIDs, dbType: DbType.String);
            param.Add("in_RecipientTypeID", value: item.RecipientTypeID, dbType: DbType.Int32);
            param.Add("in_CreatorID", value: item.CreatorID, dbType: DbType.Int32);
            param.Add("in_ChatGroupTypeId", value: item.ChatGroupTypeId, dbType: DbType.Int32);
            param.Add("in_Remark", value: item.Remark, dbType: DbType.String);

            return _baseStorage.ExecuteAsync("NCB_ChatGroupMapping_Upd", param);
        }        

        public Task<Tuple<long, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("in_BotNames", value: criteria.BotNames, dbType: DbType.String);
            param.Add("in_RecipientTypeIDs", value: criteria.RecipientTypeIDs, dbType: DbType.String);
            param.Add("in_RecipientName", value: criteria.RecipientName, dbType: DbType.String);
            param.Add("in_MappingStatusIds", value: criteria.MappingStatusIds, dbType: DbType.String);
            param.Add("in_PageIndex", value: criteria.PageIndex, dbType: DbType.Int64);
            param.Add("in_PageSize", value: criteria.PageSize, dbType: DbType.Int64);

            return _baseStorage.QueryMultipleAsync<long, BotConversation, BotConversationMember, ConversationRecipient>("NCB_ChatGroupMapping_Sel", param);
        }  
    }
}
