using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;

namespace Fanex.NCB.SkypeBot.Services.Setting
{
    public class SettingService : ISettingService
    {
        private readonly ISettingStorage _settingStorage;
        public SettingService(ISettingStorage settingStorage)
        {
            _settingStorage = settingStorage;
        }

        public async Task<List<Recipient>> GetRecipientsAsync()
        {
            return await _settingStorage.GetRecipientsAsync().ConfigureAwait(false);
        }

        public async Task<List<RecipientType>> GetRecipientTypesAsync()
        {
            return await _settingStorage.GetRecipientTypesAsync().ConfigureAwait(false);
        }
    }
}
