namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.Shared.Models;

    public interface ISettingConfigurationService
    {
        Task<(long, IEnumerable<SettingType>)> GetSettingsAsync(int pageIndex, int pageSize);

        Task<CommandResult> CreateAsync(CreateSettingType model);

        Task<SettingType> GetByTypeAsync(int type);

        Task<CommandResult> UpdateAsync(UpdateSettingType model);

        Task<CommandResult> DeleteAsync(int type, int modifiedBy);

        IEnumerable<BaseModel> GetAllSettingType();

        Task<JobInfo> GetJobInfoAsync(int jobId);
    }
}