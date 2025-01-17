namespace Fanex.NCB.Main.AppConfig
{
    using Fanex.Data.Repository;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using RemotingRef;
    using Repository;
    using Repository.Interfaces;
    using Services;
    using Services.Interfaces;
    using System.Text;

    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<ILogService, LogServiceExtend>();
            services.AddScoped<IChatGroupMappingService, ChatGroupMappingService>();
            services.AddScoped<IChatbotService, ChatbotService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<INotificationTypeService, NotificationTypeService>();
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISendNotificationService, SendNotificationService>();
            services.AddScoped<ISyncDataService, SyncDataService>();
            services.AddScoped<INotificationTemplaceService, NotificationTemplaceService>();
            services.AddScoped<IViewLogService, ViewLogService>();
            services.AddScoped<ISettingConfigurationService, SettingConfigurationService>();
            services.AddScoped<IPushSignalRService, PushSignalRService>();
            services.AddScoped<IResetLastMessageIdService, ResetLastMessageIdService>();
            services.AddScoped<IRemotingRefService, RemotingRefService>();

            services.AddHttpClient();
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDynamicRepository, DynamicRepository>();
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

        public static void RegisterConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.Configure<BotSettings>(configuration.GetSection("ChatBotConfigs"));
            services.Configure<MonitorChatbot>(configuration.GetSection("MonitorChatbot"));
            services.Configure<SignalRSettings>(configuration.GetSection("SignalRSettings"));
            services.Configure<MonitorTelegramBot>(configuration.GetSection("MonitorTelegramBot"));
        }
    }
}