namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Models.SettingConfiguration;

    public interface ISettingConfigurationService
    {
        Task<(long, IEnumerable<SettingType>)> GetSettingTypesAsync(int pageIndex, int pageSize);
    }
}