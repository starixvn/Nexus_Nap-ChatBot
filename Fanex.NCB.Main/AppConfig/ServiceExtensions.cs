namespace Fanex.NCB.Main.AppConfig
{
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Repository;
    using Fanex.NCB.Repository.Interfaces;
    using Fanex.NCB.Services;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;
    using RestSharp;

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
            services.AddScoped<ISignalRService, SignalRService>();
            services.AddScoped<IPushSignalService, PushSignalService>();

            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<ISyncDataRepository, SyncDataRepository>();
        }

        public static void RegisterJWTAuthenticate(this IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}