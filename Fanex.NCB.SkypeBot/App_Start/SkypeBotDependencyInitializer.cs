using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.MySQLStorage;
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
              //  c => new NotificationTypeService(c.GetService<INotificationTypeStorage>()));
        }

        private static void RegisterDIofMySQLStorageLayer(IServiceCollection services)
        {
            //dynamic loading DLL and register (implement later)
            services.AddTransient<INotificationTypeStorage, NotificationTypeStorage>();
        }
    }
}