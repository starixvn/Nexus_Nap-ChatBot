using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public Task<List<Recipient>> SearchRecipientsAsync(string query) => _settingStorage.SearchRecipientsAsync(query);

        public Task<List<RecipientType>> GetFilterRecipientTypesAsync() => _settingStorage.GetFilterRecipientTypesAsync();
    }
}