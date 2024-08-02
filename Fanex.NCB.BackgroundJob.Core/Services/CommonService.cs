namespace Fanex.NCB.AutoJobs.Core.Services
{
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Core.Criteria.Common;
    using Fanex.NCB.AutoJobs.Core.Criteria.SpecialDeleteTicket;
    using Fanex.NCB.AutoJobs.Core.Models.Common;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Fanex.RemotingRef;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;
        private readonly IRemotingRefService remotingRefService;

        public CommonService(IBaseService baseService, IRemotingRefService remotingRefService)
        {
            this.baseService = baseService;
            this.remotingRefService = remotingRefService;
        }

        public async Task DeleteOldLog(string currentDate)
        {
            var command = new DeleteOldConversationMessageLogCommand
            {
                CurrentDate = currentDate,
            };

            await baseService.ExecuteNonQueryAsync(command);
        }

        public string GetCategogyName(string categoryId, string languageCode)
        {
            var categoryName = remotingRefService.GetResourceDataByRefIdAndResourceId("Category", categoryId, languageCode, string.Empty);
            return categoryName;
        }

        public string GetCustomerClassName(string customerClass, string languageCode)
        {
            var customerClassName = remotingRefService.GetResourceDataByRefIdAndResourceId("CustomerClass", customerClass, languageCode, string.Empty);
            return customerClassName;
        }

        public async Task<string> GetJsonDataFromDbACCAsync(string tableName)
        {
            string jsonData = await baseService.GetDataEntityAsync<string>(new GetJsonDataOnDbACCCriteria { TableName = tableName });
            return jsonData;
        }

        public async Task<ServiceSetting> GetServiceSetting(string inServiceName)
        {
            var criteria = new GetServiceSettingCriteria
            {
                InServiceName = inServiceName
            };
            return await baseService.GetDataEntityAsync<ServiceSetting>(criteria);
        }
    }
}