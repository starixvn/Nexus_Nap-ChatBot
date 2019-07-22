namespace Fanex.NCB.Main.AppConfig
{
    using Fanex.Data.Repository;
    using Fanex.NCB.Base.Repositories;
    using Fanex.NCB.Base.Services;
    using Fanex.NCB.Base.Services.Interfaces;
    using Fanex.NCB.Repository.ChatGroupMapping;
    using Fanex.NCB.RepositoryImpl.ChatGroupMapping;
    using Fanex.NCB.Services;
    using Fanext.NCB.ServiceImpl.ChatGroupMapping;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IBaseService, BaseService>();
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