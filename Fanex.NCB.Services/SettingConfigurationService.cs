namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Core.Enum;
    using System;
    using Fanex.NCB.Core.Extensions;

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
            }).ConfigureAwait(false);

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

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }

        public async Task<SettingType> GetByTypeAsync(int type)
        {
            var result = await baseService.GetDataEntityAsync<SettingType>(new GetByTypeCriteria
            {
                Type = type
            }).ConfigureAwait(false);

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

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }

        public async Task<CommandResult> DeleteAsync(int type, int modifiedBy)
        {
            var command = new DeleteSettingTypeCommand
            {
                Type = type,
                ModifiedBy = modifiedBy
            };

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }

        public IEnumerable<BaseModel> GetAllSettingType()
        {
            var types = Enum.GetValues(typeof(SettingTypes)).Cast<SettingTypes>().Select(v => new BaseModel
            {
                Id = (int)v,
                Name = EnumHelper.GetDescription(v)
            });

            return types;
        }

        public async Task<JobInfo> GetJobInfoAsync(int jobId)
        {
            var result = await baseService.GetDataEntityAsync<JobInfo>(new GetJobInfoCriteria
            {
                JobId = jobId
            }).ConfigureAwait(false);

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

            var result = await baseService.FetchMultipleAsync<Mapping, MappingRecipients>(criteria).ConfigureAwait(false);
            var mappings = result.Item1;
            foreach (var mapping in mappings)
            {
                mapping.RecipientTypeNames = result.Item2.FirstOrDefault(mr => mr.NotificationTypeId == mapping.NotificationTypeID).RecipientTypeNames;
            }

            return (criteria.Total, mappings);
        }

        public async Task<CommandResult> UpdateJobInfoAsync(UpdateJobInfo model)
        {
            var command = new UpdateJobInfoCommand
            {
                Id = model.Id,
                BatchSize = model.BatchSize,
                TestingGroup = model.TestingGroup,
                ModifiedBy = model.ModifiedBy
            };

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }

        public async Task<IEnumerable<SettingType>> GetMsgContentsTypesAsync()
        {
            var result = await baseService.GetDataAsync<SettingType>(new GetMsgContentsTypesCriteria());

            return result;
        }

        public async Task<NotificationTypeData> GetNotificationTypeDataAsync(int notificationTypeId)
        {
            var result = await baseService.GetDataEntityAsync<NotificationTypeData>(new GetNotificationTypeDataCriteria
            {
                NotificationTypeId = notificationTypeId
            });

            return result;
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

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

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
                NotificationTypeId = model.NotificationTypeID,
                NotificationTemplateId = model.NotificationTemplateID,
                MsgContentsType = model.MsgContentsType,
                RequiredParam = model.RequiredParams,
                IsTest = model.IsTest,
                OldMsgContentsType = model.OldMsgContentsType,
                ModifiedBy = model.ModifiedBy
            };

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

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

            var result = await baseService.ExecuteNonQueryAsync(command).ConfigureAwait(false);

            return result;
        }
    }
}