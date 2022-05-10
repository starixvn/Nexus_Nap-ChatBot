namespace Fanex.NCB.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Criteria.Common;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models.OIC;
    using Fanex.NCB.Shared.Services.Interfaces;
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

        public async Task<IEnumerable<NotificationType>> GetNotificationTypesAsync()
        {
            return await baseService.GetDataAsync<NotificationType>(new GetNotificationTypesCriteria());
        }

        public async Task<IEnumerable<Models.Common.Product>> GetProductsAsync()
        {
            return await baseService.GetDataAsync<Models.Common.Product>(new GetProductsCriteria());
        }

        public async Task<IEnumerable<NotificationStatus>> GetNotificationStatusesAsync()
        {
            return await baseService.GetDataAsync<NotificationStatus>(new GetNotificationStatusCriteria());
        }

        public async Task<IEnumerable<Recipient>> GetRecipientsAsync()
        {
            return await baseService.GetDataAsync<Recipient>(new GetRecipientsCriteria());
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await baseService.GetDataAsync<User>(new GetUsersCriteria());
        }

        public async Task<IEnumerable<ConversationChannel>> GetConversationChannelAsync()
        {
            return await baseService.GetDataAsync<ConversationChannel>(new GetConversationChannelCriteria());
        }

        public Task<IEnumerable<Platform>> GetPlatformAsync(int notificationTypeId)
        {
            var criteria = new GetPlatformCriteria
            {
                NotificationTypeId = notificationTypeId
            };
            return baseService.GetDataAsync<Platform>(criteria);
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
    }
}