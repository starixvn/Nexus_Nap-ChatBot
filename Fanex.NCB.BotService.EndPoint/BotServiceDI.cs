namespace Fanex.NCB.BotService.EndPoint
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using Fanex.Data.Repository;
    using Fanex.NCB.BotService.Core;
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

    public static class BotServiceDI
    {
        public static void Init(IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment, AppSettings appSettings, ILogger logger)
        {
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IDynamicRepository, DynamicRepository>();
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IConversationService, ConversationService>();
            services.AddTransient<ICredentialProvider, MultiCredentialProvider>();
            services.AddTransient<IBot, LicBot>();
            services.AddTransient<Logging.ILogger, Logging.Logger>();
            var botConfig = GetBotConfig(environment, appSettings, logger);
            services.AddSingleton(botConfig);
            services.AddSingleton(appSettings);

            var botFrameworkHttpAdapter = GetBotFrameworkHttpAdapter(botConfig, configuration);
            services.AddSingleton(botFrameworkHttpAdapter);

            services.AddBot<LicBot>(options =>
            {
                options.CredentialProvider = new MultiCredentialProvider(botConfig);
                options.HttpClient = GetDefaultHttpClient(configuration);
                options.OnTurnError = (turnContext, exception) =>
                {
                    try
                    {
                        logger.LogError($"{DateTime.UtcNow}: Exception caught : {exception}");
                    }
                    catch
                    { }

                    throw exception;
                };
            });
        }

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
                };

                httpClient = new HttpClient(handler: socketsHttpHandler);
                httpClient.Timeout = TimeSpan.FromSeconds(htttClientTimeOut);
            }
            else
            {
                httpClient = new HttpClient();
            }

            return httpClient;
        }

        private static BotConfiguration GetBotConfig(IHostingEnvironment environment, AppSettings appSettings, ILogger logger)
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

                logger.LogCritical(msg, ex);
                throw new InvalidOperationException(msg);
            }

            return botConfig;
        }
    }
}