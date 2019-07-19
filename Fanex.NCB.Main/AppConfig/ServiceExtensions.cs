namespace Fanex.NCB.Main.AppConfig
{
    using Fanex.NCB.Repository.ChatGroupMapping;
    using Fanex.NCB.RepositoryImpl.ChatGroupMapping;
    using Fanex.NCB.Services;
    using Fanext.NCB.ServiceImpl.ChatGroupMapping;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IChatGroupMappingService, ChatGroupMappingService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IChatGroupMappingRepository, ChatGroupMappingRepository>();
        }
    }
}