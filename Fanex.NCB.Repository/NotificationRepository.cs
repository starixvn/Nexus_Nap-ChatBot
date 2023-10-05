namespace Fanex.NCB.Repository
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Shared.Models.IntegrationMethod;
    using Fanex.NCB.Shared.Models.OIC;

    public class NotificationRepository : INotificationRepository
    {
        public async Task<EditNotificationRepository> FetchEditNotificationInfo(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var data = new EditNotificationRepository
                    {
                        EditNotificationInfos = multi.Read<EditNotificationInfo>(),
                        Recipients = multi.Read<Recipient>(),
                        Platforms = multi.Read<Platform>(),
                        IntegrationMethods = multi.Read<IntegrationMethodInfo>()
                    };

                    return data;
                }
            }
        }

        public async Task<ResendNotificationRepository> FetchResendNotificationInfo(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var data = new ResendNotificationRepository
                    {
                        ResendNotifications = multi.Read<ResendNotification>(),
                        Recipients = multi.Read<Recipient>(),
                        Platforms = multi.Read<Platform>(),
                        IntegrationMethods = multi.Read<IntegrationMethodInfo>()
                    };

                    return data;
                }
            }
        }
    }
}
