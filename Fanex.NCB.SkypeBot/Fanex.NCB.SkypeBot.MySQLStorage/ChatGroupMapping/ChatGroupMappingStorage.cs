using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;

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
            param.Add("groupID", value: item.GroupID, dbType: DbType.Int32);
            param.Add("recipientIDs", value: item.RecipientIDs, dbType: DbType.String);
            param.Add("recipientTypeID", value: item.RecipientTypeID, dbType: DbType.Int32);
            param.Add("creatorID", value: item.CreatorID, dbType: DbType.Int32);
            param.Add("chatGroupTypeId", value: item.ChatGroupTypeId, dbType: DbType.Int32);
            param.Add("remark", value: item.Remark, dbType: DbType.String);

            return _baseStorage.ExecuteAsync("NCB_ChatGroupMapping_Upd", param);
        }        

        public Task<Tuple<long, List<BotConversation>, List<BotConversationMember>, List<ConversationRecipient>>> GetChatGroupMappingListAsync(GetChapGroupMappingCriteria criteria)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("botNames", value: criteria.BotNames, dbType: DbType.String);
            param.Add("recipientTypeIDs", value: criteria.RecipientTypeIDs, dbType: DbType.String);
            param.Add("recipientName", value: criteria.RecipientName, dbType: DbType.String);
            param.Add("mappingStatusIds", value: criteria.MappingStatusIds, dbType: DbType.String);
            param.Add("licenseeStatusIds", value: criteria.LicenseeStatusIds, dbType: DbType.String);
            param.Add("pageIndex", value: criteria.PageIndex, dbType: DbType.Int64);
            param.Add("pageSize", value: criteria.PageSize, dbType: DbType.Int64);

            return _baseStorage.QueryMultipleAsync<long, BotConversation, BotConversationMember, ConversationRecipient>("NCB_ChatGroupMapping_Sel", param);
        }  
    }
}
