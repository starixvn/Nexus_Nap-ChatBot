namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.ProblemAccount;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models.ProblemAccounts;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class ProblemAccountService : IProblemAccountService
    {
        private readonly IBaseService baseService;

        public ProblemAccountService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<MessageSetting>> GetMessageSetting()
        {
            var criteria = new GetMessageSettingCriteria();
            return await baseService.GetDataAsync<MessageSetting>(criteria);
        }
    }
}
