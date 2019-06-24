using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Services.Setting
{
    public interface ISettingService
    {
        Task<List<RecipientType>> GetRecipientTypesAsync();

        Task<List<RecipientType>> GetFilterRecipientTypesAsync();

        Task<List<Recipient>> GetRecipientsAsync();

        Task<List<BotName>> GetBotNameAsync();

        Task<List<ChatGroupType>> GetChatGroupTypeAsync();

        Task<List<LicenseeStatus>> GetLicenseeStatusAsync();

        Task<List<Recipient>> SearchRecipientsAsync(string query);
    }
}