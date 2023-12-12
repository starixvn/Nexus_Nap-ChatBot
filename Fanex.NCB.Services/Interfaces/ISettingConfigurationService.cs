namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.Shared.Models;

    public interface ISettingConfigurationService
    {
        Task<(long, IEnumerable<SettingType>)> GetSettingsAsync(int pageIndex, int pageSize, int? settingType);

        Task<CommandResult> CreateAsync(CreateSettingType model);

        Task<SettingType> GetByTypeAsync(int type);

        Task<CommandResult> UpdateAsync(UpdateSettingType model);

        Task<CommandResult> DeleteAsync(int type, int modifiedBy);

        Task<IEnumerable<BaseModel>> GetAllSettingType();

        Task<JobInfo> GetJobInfoAsync(int jobId);

        Task<(long, IEnumerable<Mapping>)> GetMappingsAsync(int jobId, int pageIndex, int pageSize);

        Task<CommandResult> UpdateJobInfoAsync(UpdateJobInfo model, int modifiedBy);

        Task<IEnumerable<SettingType>> GetMsgContentsTypesAsync();

        Task<NotificationTypeData> GetNotificationTypeDataAsync(int notificationTypeId);

        Task<CommandResult> CreateMappingAsync(CreateMapping model);

        Task<Mapping> GetMappingAsync(GetMapping model);

        Task<CommandResult> UpdateMappingAsync(UpdateMapping model);

        Task<CommandResult> DeleteMappingAsync(DeleteMapping model);
    }
}