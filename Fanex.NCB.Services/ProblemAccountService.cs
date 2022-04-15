namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Criteria.ProblemAccount;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.ProblemAccounts;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class ProblemAccountService : IProblemAccountService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public ProblemAccountService(IBaseService baseService, ICommonService commonService)
        {
            this.baseService = baseService;
            this.commonService = commonService;
        }

        public async Task<MessageSetting> GetMessageSettingAsync()
        {
            var criteria = new GetMessageSettingCriteria();
            return await baseService.GetDataEntityAsync<MessageSetting>(criteria);
        }

        public async Task<(IEnumerable<ProblemAccountInfo>, IEnumerable<short>)> GetProblemAccountAsync()
        {
            var messageSetting = await GetMessageSettingAsync();
            var criteria = new GetProblemAccountInfoCriteria
            {
                BatchSize = messageSetting.BatchSize,
                LastLogId = messageSetting.LastLogId
            };
            return await baseService.FetchMultipleAsync<ProblemAccountInfo, short>(criteria);
        }

        public IEnumerable<Category> GetAllCategory(IEnumerable<short> categoriesId)
        {
            var categories = new List<Category>();
            var category = new Category();
            foreach (int categoryId in categoriesId)
            {
                var categoryNameEN = commonService.GetCategogyName(categoryId.ToString(), Language.en_US);
                var categoryNameCS = commonService.GetCategogyName(categoryId.ToString(), Language.zh_CN);
                category.CategoryId = categoryId.ToString();
                category.CategoryNameEN = categoryNameEN;
                category.CategoryNameCS = categoryNameCS;
                categories.Add(category);
            }
            return categories;
        }

        public async Task<CommandResult> CreateFromProblemAccountAsync()
        {
            var (message, categoriesId) = await GetProblemAccountAsync();
            var command = new CreateFromProblemAccountCriteria
            {
                Message = JsonConvert.SerializeObject(message),
                CategoryInfo = JsonConvert.SerializeObject(GetAllCategory(categoriesId))
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}
