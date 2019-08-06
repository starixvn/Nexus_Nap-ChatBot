namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.Common;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;

        public CommonService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<NotificationType>> GetNotificationTypesAsync()
        {
            return await baseService.GetDataAsync<NotificationType>(new GetNotificationTypesCriteria()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await baseService.GetDataAsync<Product>(new GetProductsCriteria()).ConfigureAwait(false);
        }
    }
}