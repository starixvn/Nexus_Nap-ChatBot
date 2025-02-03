namespace Fanex.NCB.Services.Interfaces;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.NCB.Core.Models;
using Models.Setting;

public interface ISettingService
{
    Task<IEnumerable<BotName>> GetBotNameAsync();

    Task<IEnumerable<BaseModel>> GetRecipientTypesAsync();

    Task<IEnumerable<ChatGroupType>> GetChatGroupTypeAsync();

    Task<IEnumerable<BaseModel>> GetLicenseeStatusAsync();

    Task<IEnumerable<Recipient>> SearchRecipientsAsync(string query);

    Task<IEnumerable<Recipient>> GetRecipientsAsync();

    Task<IEnumerable<BaseModel>> GetFilterRecipientTypesAsync();

    Task<IEnumerable<BaseModel>> SearchChatGroupAsync(string query);

    IEnumerable<BaseModel> GetAllMappingStatus();

    Task<IEnumerable<BaseModel>> GetSystemAsync();
}