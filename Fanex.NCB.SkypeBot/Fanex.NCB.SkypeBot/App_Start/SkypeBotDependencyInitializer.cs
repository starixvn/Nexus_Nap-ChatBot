using Fanex.NCB.Shared.MySQLExtension.Storage;
using Fanex.NCB.SkypeBot.BaseStorage;
using Fanex.NCB.SkypeBot.MySQLStorage.ChatGroupMapping;
using Fanex.NCB.SkypeBot.MySQLStorage.NotificationType;
using Fanex.NCB.SkypeBot.MySQLStorage.Setting;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMappingResultService;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
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
            services.AddTransient<IChatGroupMappingService, ChatGroupMappingService>();
            services.AddTransient<IMappingStatusService, MappingStatusService>();
            services.AddTransient<IChatGroupMappingResultService, ChatGroupMappingResultService>();
        }

        private static void RegisterDIofMySQLStorageLayer(IServiceCollection services)
        {
            //dynamic loading DLL and register (implement later)
            services.AddTransient<INotificationTypeStorage, NotificationTypeStorage>();
            services.AddTransient<IChatGroupMappingStorage, ChatGroupMappingStorage>();
            services.AddTransient<ISettingStorage, SettingStorage>();
            services.AddTransient<IBaseStorage, Shared.MySQLExtension.Storage.BaseStorage>();
        }
    }
}