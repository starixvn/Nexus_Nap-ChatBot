namespace Fanex.NCB.Main.AppConfig
{
    using Fanex.Data.Repository;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Base.Repositories;
    using Fanex.NCB.Base.Services;
    using Fanex.NCB.Base.Services.Interfaces;
    using Fanex.NCB.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services.Interfaces;
    using Fanext.NCB.Services;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IChatGroupMappingService, ChatGroupMappingService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddScoped<IChatGroupMappingRepository, ChatGroupMappingRepository>();
        }
    }
}