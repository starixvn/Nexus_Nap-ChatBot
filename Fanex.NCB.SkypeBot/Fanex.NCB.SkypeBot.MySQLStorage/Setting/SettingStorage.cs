using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System;
using System.Collections.Generic;
using System.Text;
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
            return _baseStorage.QueryAsync<Recipient>("NCB_Recipient_Sel");
        }

        public Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return _baseStorage.QueryAsync<RecipientType>("NCB_RecipientType_Sel");
        }
    }
}
