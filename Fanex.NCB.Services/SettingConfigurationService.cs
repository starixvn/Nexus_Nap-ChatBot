namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class SettingConfigurationService : ISettingConfigurationService
    {
        private readonly IBaseService baseService;

        public SettingConfigurationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<(long, IEnumerable<SettingType>)> GetSettingTypesAsync(int pageIndex, int pageSize)
        {
            var result = await baseService.FetchMultipleAsync<long, SettingType>(new GetSettingTypeListCriteria
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            }).ConfigureAwait(false);

            return (result.Item1.FirstOrDefault(), result.Item2);
        }
    }
}