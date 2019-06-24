using Dapper;
using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.MySQLStorage.Setting
{
    public class SettingStorage : ISettingStorage
    {
        private readonly IBaseStorage _baseStorage;

        public SettingStorage(IBaseStorage baseStorage)
        {
            _baseStorage = baseStorage;
        }

        public Task<List<Recipient>> GetRecipientsAsync()
        {
            return _baseStorage.QueryAsync<Recipient>("NCB_Setting_Recipient_Sel");
        }

        public Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return _baseStorage.QueryAsync<RecipientType>("NCB_Common_RecipientType_Sel");
        }

        public Task<List<BotName>> GetBotNameAsync()
        {
            return _baseStorage.QueryAsync<BotName>("NCB_Setting_BotName_Sel");
        }

        public Task<List<ChatGroupType>> GetChatGroupTypeAsync()
        {
            return _baseStorage.QueryAsync<ChatGroupType>("NCB_Setting_ChatGroupType_Sel");
        }

        public Task<List<LicenseeStatus>> GetLicenseeStatusAsync()
        {
            return _baseStorage.QueryAsync<LicenseeStatus>("NCB_Setting_LicenseeStatus_Sel");
        }

        public Task<List<Recipient>> SearchRecipientsAsync(string query)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("query", value: query, dbType: DbType.String);

            return _baseStorage.QueryAsync<Recipient>("NCB_Setting_SearchRecipient_Sel", param);
        }

        public Task<List<RecipientType>> GetFilterRecipientTypesAsync()
        {
            return _baseStorage.QueryAsync<RecipientType>("NCB_Setting_RecipientType_Sel");
        }
    }
}