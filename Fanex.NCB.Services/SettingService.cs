namespace Fanex.NCB.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enum;
using Core.Extensions;
using Fanex.NCB.Core.Models;
using Criteria.NotificationType;
using Criteria.Setting;
using Models.Setting;
using Interfaces;
using Fanex.NCB.Shared.Services.Interfaces;

public class SettingService : ISettingService
{
    private readonly IBaseService baseService;

    public SettingService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public IEnumerable<BaseModel> GetAllMappingStatus()
    {
        var status = Enum.GetValues(typeof(MappingStatus)).Cast<MappingStatus>().Select(v => new BaseModel
        {
            Id = (int)v,
            Name = EnumHelper.GetDescription(v)
        });
        return status;
    }

    public Task<IEnumerable<BotName>> GetBotNameAsync()
    {
        var criteria = new GetBotNameCriteria();
        return baseService.GetDataAsync<BotName>(criteria);
    }

    public Task<IEnumerable<ChatGroupType>> GetChatGroupTypeAsync()
    {
        var criteria = new GetChatGroupTypeCriteria();
        return baseService.GetDataAsync<ChatGroupType>(criteria);
    }

    public Task<IEnumerable<BaseModel>> GetFilterRecipientTypesAsync()
    {
        var criteria = new GetFilterRecipientTypeCriteria();
        return baseService.GetDataAsync<BaseModel>(criteria);
    }

    public Task<IEnumerable<BaseModel>> GetLicenseeStatusAsync()
    {
        var criteria = new GetLicenseeStatusCriteria();
        return baseService.GetDataAsync<BaseModel>(criteria);
    }

    public Task<IEnumerable<Recipient>> GetRecipientsAsync()
    {
        var criteria = new GetRecipientCriteria();
        return baseService.GetDataAsync<Recipient>(criteria);
    }

    public Task<IEnumerable<BaseModel>> GetRecipientTypesAsync()
    {
        var criteria = new GetRecipientTypeCriteria();
        return baseService.GetDataAsync<BaseModel>(criteria);
    }

    public Task<IEnumerable<BaseModel>> SearchChatGroupAsync(string query)
    {
        var criteria = new SearchChatGroupNameCriteria
        {
            Keyword = query
        };
        return baseService.GetDataAsync<BaseModel>(criteria);
    }

    public Task<IEnumerable<Recipient>> SearchRecipientsAsync(string query)
    {
        var criteria = new SearchRecipientCriteria
        {
            Keyword = query
        };
        return baseService.GetDataAsync<Recipient>(criteria);
    }

    public Task<IEnumerable<BaseModel>> GetSystemAsync()
    {
        var criteria = new GetNotificationTypeSystemCriteria();

        return baseService.GetDataAsync<BaseModel>(criteria);
    }
}