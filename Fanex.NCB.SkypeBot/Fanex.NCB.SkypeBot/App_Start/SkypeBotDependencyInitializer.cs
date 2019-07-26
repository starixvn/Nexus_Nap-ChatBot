using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.MySQLStorage.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.NCB.SkypeBot.App_Start
{
    public static class SkypeBotDependencyInitializer
    {
        public static void Initialize(IServiceCollection services)
        {
            RegisterDIofMySQLStorageLayer(services);
            services.AddTransient<INotificationTypeService, NotificationTypeService>();
        }

        private static void RegisterDIofMySQLStorageLayer(IServiceCollection services)
        {
            services.AddTransient<INotificationTypeStorage, NotificationTypeStorage>();
            services.AddTransient<IBaseStorage, Shared.MySQLExtension.Storage.BaseStorage>();
        }
    }
}