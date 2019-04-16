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

        public Task<List<Recipient>> GetRecipientsAsync() => _settingStorage.GetRecipientsAsync();

        public Task<List<RecipientType>> GetRecipientTypesAsync() => _settingStorage.GetRecipientTypesAsync();        

        public Task<List<BotName>> GetBotNameAsync() => _settingStorage.GetBotNameAsync();
        
        public Task<List<ChatGroupType>> GetChatGroupTypeAsync() => _settingStorage.GetChatGroupTypeAsync();

        public Task<List<LicenseeStatus>> GetLicenseeStatusAsync() => _settingStorage.GetLicenseeStatusAsync();
    }
}
