namespace Fanex.NCB.BotService.EndPoint
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Authentication;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using StackExchange.Redis;

    public static class BotServiceDI
    {
        [Obsolete]
        public static void Init(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment, AppSettings appSettings, ILogger logger)
        {
            ConfigHttpClient(services, configuration);
            services.Configure<ProxySettings>(options => configuration.GetSection("ProxySettings").Bind(options));
            services.Configure<SendIndirectMessageSettings>(options => configuration.GetSection("SendIndirectMessageSettings").Bind(options));

            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IConversationService, ConversationService>();
            services.AddSingleton<ISkypeBotService, SkypeBotService>();
            services.AddSingleton<IMessageLogService, MessageLogService>();
            services.AddSingleton<IRedisCacheExtendService, RedisCacheExtendService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<ICredentialProvider, MultiCredentialProvider>();
            services.AddSingleton<IBot, LicBot>();
            services.AddSingleton<Logging.ILogger, Logging.Logger>();
            var botConfig = GetBotConfig(environment, appSettings, logger);
            services.AddSingleton(botConfig);
            services.AddSingleton(appSettings);

            RegisterCheckDuplicateMessageLogSettings(services, configuration);
            RegisterRedisConnection(services, configuration);
            var botFrameworkHttpAdapter = GetBotFrameworkHttpAdapter(botConfig, configuration);
            services.AddSingleton(botFrameworkHttpAdapter);

            services.AddBot<LicBot>(options =>
            {
                options.CredentialProvider = new MultiCredentialProvider(botConfig);
                options.HttpClient = GetDefaultHttpClient(configuration);
                options.OnTurnError = (turnContext, exception) =>
                {
                    throw exception;
                };
            });

            RegisterTelegramBots(services, configuration);
            services.AddScoped<ITelegramBotService, TelegramBotService>();
        }

        [Obsolete]
        private static IBotFrameworkHttpAdapter GetBotFrameworkHttpAdapter(BotConfiguration botConfig, IConfiguration configuration)
        {
            var credentialProvider = new MultiCredentialProvider(botConfig);
            HttpClient httpClient = GetDefaultHttpClient(configuration);
            IBotFrameworkHttpAdapter botFrameworkHttpAdapter = new BotFrameworkHttpAdapter(credentialProvider, null, httpClient, null);

            return botFrameworkHttpAdapter;
        }

        private static HttpClient GetDefaultHttpClient(IConfiguration configuration)
        {
            var isUseProxy = bool.Parse(configuration.GetSection("ProxySettings:IsUseProxy").Value);
            var proxyAddress = configuration.GetSection("ProxySettings:ProxyAddress").Value;
            var bypassOnLocal = bool.Parse(configuration.GetSection("ProxySettings:BypassOnLocal").Value);
            var htttClientTimeOut = double.Parse(configuration.GetSection("ProxySettings:TimeOut").Value);

            HttpClient httpClient;

            if (isUseProxy)
            {
                var proxy = new WebProxy(proxyAddress, bypassOnLocal);
                var socketsHttpHandler = new SocketsHttpHandler
                {
                    UseProxy = isUseProxy,
                    Proxy = proxy,
                    SslOptions = new SslClientAuthenticationOptions
                    {
                        EnabledSslProtocols = SslProtocols.Tls12
                    }
                };

                httpClient = new HttpClient(handler: socketsHttpHandler)
                {
                    Timeout = TimeSpan.FromSeconds(htttClientTimeOut)
                };
            }
            else
            {
                httpClient = new HttpClient();
            }

            return httpClient;
        }

        [Obsolete]
        private static BotConfiguration GetBotConfig(IWebHostEnvironment environment, AppSettings appSettings, ILogger logger)
        {
            BotConfiguration botConfig = null;
            try
            {
                var realPath = Path.Combine(environment.ContentRootPath, appSettings.BotFilePath);
                botConfig = BotConfiguration.Load(realPath, appSettings.BotFileSecret);
            }
            catch (Exception ex)
            {
                const string msg = @"Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.
                    - You can find the botFilePath and botFileSecret in the Azure App Service application settings.
                    - If you are running this bot locally, consider adding a appsettings.json file with botFilePath and botFileSecret.
                    - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.";

                throw new InvalidOperationException(msg, ex);
            }

            return botConfig;
        }

        private static void RegisterCheckDuplicateMessageLogSettings(IServiceCollection services, IConfiguration configuration)
        {
            var settingConfiguration = configuration.GetSection("CheckDuplicateMessageLogSettings");
            services.Configure<CheckDuplicatedMessageLogSetting>(settingConfiguration);
            var checkDuplicatedMessageSettings = settingConfiguration.Get<CheckDuplicatedMessageLogSetting>();
            services.AddSingleton(checkDuplicatedMessageSettings);
        }

        private static void RegisterRedisConnection(IServiceCollection services, IConfiguration configuration)
        {
            var settingConfiguration = configuration.GetSection("Fanex.Caching.Redis");
            services.Configure<RedisConfiguration>(settingConfiguration);
            var redisConfig = settingConfiguration.Get<RedisConfiguration>();
            redisConfig.Db = GetDbNumber(redisConfig.ConnectionString);
            services.AddSingleton(redisConfig);

            var multiplexer = ConnectionMultiplexer.Connect(redisConfig.ConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        }

        private static void RegisterTelegramBots(IServiceCollection services, IConfiguration configuration)
        {
            var configs = configuration.GetSection("TelegramBotConfigurations");
            services.Configure<List<TelegramBotConfigurations>>(configs);
            var bots = configs.Get<List<TelegramBotConfigurations>>();

            services.AddSingleton(bots);
        }

        private static void ConfigHttpClient(IServiceCollection services, IConfiguration configuration)
        {
            var isUseProxy = bool.Parse(configuration.GetSection("ProxySettings:IsUseProxy").Value);
            if (isUseProxy)
            {
                var proxyAddress = configuration.GetSection("ProxySettings:ProxyAddress").Value;
                var bypassOnLocal = bool.Parse(configuration.GetSection("ProxySettings:BypassOnLocal").Value);
                services.AddHttpClient(NamedHttpClients.ProxiedClient)
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var proxy = new WebProxy(proxyAddress, bypassOnLocal);
                        return new SocketsHttpHandler
                        {
                            UseProxy = isUseProxy,
                            Proxy = proxy,
                            SslOptions = new SslClientAuthenticationOptions
                            {
                                EnabledSslProtocols = SslProtocols.Tls12
                            }
                        };
                    });
            }
            else
            {
                services.AddHttpClient();
            }
        }

        private static int GetDbNumber(string cnn)
        {
            var db = BetweenStrings(cnn, "defaultDatabase=", ",");
            return int.Parse(db);
        }

        private static string BetweenStrings(string text, string start, string end)
        {
            int p1 = text.IndexOf(start) + start.Length;
            int p2 = text.IndexOf(end, p1);
            if (p2 == -1 || end == string.Empty)
            {
                return text.Substring(p1);
            }
            return text.Substring(p1, p2 - p1);
        }
    }
}