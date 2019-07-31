namespace Fanex.NCB.Main.AppConfig
{
    using Fanex.Data.Repository;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Repositories;
    using Fanex.NCB.Core.Services;
    using Fanex.NCB.Core.Services.Interfaces;
    using Fanex.NCB.Services;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IChatGroupMappingService, ChatGroupMappingService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<INotificationTypeService, NotificationTypeService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
        }
    }
}