namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Base.Models;
    using Fanex.NCB.Models.Setting;

    public interface ISettingService
    {
        Task<IEnumerable<BotName>> GetBotNameAsync();

        Task<IEnumerable<BaseModel>> GetRecipientTypesAsync();

        Task<IEnumerable<ChatGroupType>> GetChatGroupTypeAsync();

        Task<IEnumerable<BaseModel>> GetLicenseeStatusAsync();

        Task<IEnumerable<Recipient>> SearchRecipientsAsync(string query);

        Task<IEnumerable<Recipient>> GetRecipientsAsync();

        Task<IEnumerable<BaseModel>> GetFilterRecipientTypesAsync();

        IEnumerable<BaseModel> GetAllMappingStatus();
    }
}