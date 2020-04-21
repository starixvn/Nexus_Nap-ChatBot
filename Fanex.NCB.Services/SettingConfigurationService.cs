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

        public async Task<(long, IEnumerable<Mapping>)> GetMappingAsync(int jobId, int pageIndex, int pageSize)
        {
            var result = await baseService.FetchMultipleAsync<long, Mapping>(new GetMappingCriteria
            {
                JobId = jobId,
                PageIndex = pageIndex,
                PageSize = pageSize
            }).ConfigureAwait(false);

            return (result.Item1.FirstOrDefault(), result.Item2);
        }
    }
}