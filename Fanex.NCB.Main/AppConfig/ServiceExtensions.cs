namespace Fanex.NCB.Main.AppConfig
{
    using Fanex.Data.Repository;
    using Fanex.NCB.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IChatbotService, ChatbotService>();
            services.AddScoped<IChatGroupMappingService, ChatGroupMappingService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<INotificationTypeService, NotificationTypeService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISendNotificationService, SendNotificationService>();
            services.AddScoped<ISyncDataService, SyncDataService>();
            services.AddScoped<INotificationTemplaceService, NotificationTemplaceService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<ISyncDataRepository, SyncDataRepository>();
        }
    }
}