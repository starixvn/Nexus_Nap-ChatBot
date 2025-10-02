namespace Fanex.NCB.Services;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Constants;
using Fanex.NCB.Core.Models;
using Criteria.SettingConfiguration;
using Models.Notification;
using Models.SettingConfiguration;
using Interfaces;
using Fanex.NCB.Shared.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using Newtonsoft.Json;

public class SettingConfigurationService : ISettingConfigurationService
{
    private readonly IBaseService baseService;

    public SettingConfigurationService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public async Task<(long, IEnumerable<SettingType>)> GetSettingsAsync(int pageIndex, int pageSize, int? settingType)
    {
        var result = await baseService.FetchMultipleAsync<long, SettingType>(new GetSettingTypeListCriteria
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            SettingType = settingType
        });

        return (result.Item1.FirstOrDefault(), result.Item2);
    }

    public async Task<CommandResult> CreateAsync(CreateSettingType model)
    {
        var command = new CreateSettingTypeCommand
        {
            Type = model.Type,
            Name = model.Name,
            CreatorId = model.CreatorId
        };

        var result = await baseService.ExecuteNonQueryAsync(command);

        return result;
    }

    public async Task<SettingType> GetByTypeAsync(int type)
    {
        var result = await baseService.GetDataEntityAsync<SettingType>(new GetByTypeCriteria
        {
            Type = type
        });

        return result;
    }

    public async Task<CommandResult> UpdateAsync(UpdateSettingType model)
    {
        var command = new UpdateSettingTypeCommand
        {
            Type = model.Type,
            Name = model.Name,
            ModifiedBy = model.ModifiedBy
        };

        var result = await baseService.ExecuteNonQueryAsync(command);

        return result;
    }

    public async Task<CommandResult> DeleteAsync(int type, int modifiedBy)
    {
        var command = new DeleteSettingTypeCommand
        {
            Type = type,
            ModifiedBy = modifiedBy
        };

        var result = await baseService.ExecuteNonQueryAsync(command);

        return result;
    }

    public async Task<IEnumerable<BaseModel>> GetAllSettingType()
    {
        var types = await baseService.GetDataAsync<BaseModel>(new GetAllSettingTypeCriteria());

        return types;
    }

    public async Task<JobInfo> GetJobInfoAsync(int jobId)
    {
        var result = await baseService.GetDataEntityAsync<JobInfo>(new GetJobInfoCriteria
        {
            JobId = jobId
        });

        return result;
    }

    public async Task<(long, IEnumerable<Mapping>)> GetMappingsAsync(int jobId, int pageIndex, int pageSize)
    {
        var criteria = new GetMappingsCriteria
        {
            JobId = jobId,
            PageIndex = pageIndex,
            PageSize = pageSize
        };

        var result = await baseService.FetchMultipleAsync<Mapping, MappingRecipients>(criteria);
        var mappings = result.Item1;
        foreach (var mapping in mappings)
        {
            mapping.RecipientTypeNames = result.Item2
                .FirstOrDefault(mr => mr.NotificationTypeId == mapping.NotificationTypeID)?.RecipientTypeNames;
        }

        return (criteria.Total, mappings);
    }

    public async Task<CommandResult> UpdateJobInfoAsync(UpdateJobInfo model, int modifiedBy)
    {
        var jobInfo = new List<UpdateJobSettingInfo>();
        foreach (var property in model.GetType().GetProperties())
        {
            if (property.Name != Constants.JobIdentify)
            {
                jobInfo.Add(new UpdateJobSettingInfo
                {
                    JobIdentify = model.JobIdentify,
                    SettingName = property.Name,
                    SettingValue = property.GetValue(model).ToString()
                });
            }
        }

        var command = new UpdateJobInfoCommand
        {
            JobInfo = JsonConvert.SerializeObject(jobInfo),
            ModifiedBy = modifiedBy
        };

        return await baseService.ExecuteNonQueryAsync(command);
    }

    public async Task<IEnumerable<SettingType>> GetMsgContentsTypesAsync()
    {
        var result = await baseService.GetDataAsync<SettingType>(new GetMsgContentsTypesCriteria());

        return result;
    }

    public async Task<NotificationTypeData> GetNotificationTypeDataAsync(int notificationTypeId)
    {
        NotificationTypeData data = null;
        var result = await baseService.FetchMultipleAsync<NotificationTypeData, Template>(
            new GetNotificationTypeDataCriteria
            {
                NotificationTypeId = notificationTypeId
            });

        if (result.Item1 != null)
        {
            data = result.Item1.FirstOrDefault();
            if (data is not null)
            {
                data.Templates = result.Item2;
            }
        }

        return data;
    }

    public async Task<CommandResult> CreateMappingAsync(CreateMapping model)
    {
        var command = new CreateMappingCommand
        {
            JobId = model.JobId,
            NotificationTypeId = model.NotificationTypeID,
            NotificationTemplateId = model.NotificationTemplateID,
            MsgContentsType = model.MsgContentsType,
            RequiredParam = model.RequiredParams,
            IsTest = model.IsTest,
            CreatorId = model.CreatorId
        };

        var result = await baseService.ExecuteNonQueryAsync(command);

        return result;
    }

    public async Task<Mapping> GetMappingAsync(GetMapping model)
    {
        var result = await baseService.GetDataEntityAsync<Mapping>(new GetMappingCriteria
        {
            MsgContentsType = model.MsgContentsType,
            JobId = model.JobId
        });

        return result;
    }

    public async Task<CommandResult> UpdateMappingAsync(UpdateMapping model)
    {
        var command = new UpdateMappingCommand
        {
            JobId = model.JobId,
            NewNotificationTypeId = model.NotificationTypeID,
            NewNotificationTemplateId = model.NotificationTemplateID,
            NewMsgContentsType = model.MsgContentsType,
            NewRequiredParam = model.RequiredParams,
            NewIsTest = model.IsTest,
            OldMsgContentsType = model.OldMsgContentsType,
            OldNotificationTemplateId = model.OldNotificationTemplateId,
            OldNotificationTypeId = model.OldNotificationTypeId,
            ModifiedBy = model.ModifiedBy
        };

        var result = await baseService.ExecuteNonQueryAsync(command);

        return result;
    }

    public async Task<CommandResult> DeleteMappingAsync(DeleteMapping model)
    {
        var command = new DeleteMappingCommand
        {
            JobId = model.JobId,
            NotificationTemplateId = model.TemlateId,
            MsgContentsType = model.MsgContentsType,
            ModifiedBy = model.ModifiedBy
        };

        var result = await baseService.ExecuteNonQueryAsync(command);

        return result;
    }
}