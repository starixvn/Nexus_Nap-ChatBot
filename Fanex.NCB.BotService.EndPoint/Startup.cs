namespace Fanex.NCB.BotService.EndPoint
{
    using Fanex.NCB.BotService.EndPoint.Bots;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;

    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = env;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            BotServiceDI.Init(services);
            LoadBotConfig(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();

            app.UseMvc();
        }

        private void LoadBotConfig(IServiceCollection services)
        {
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            BotConfiguration botConfig = null;
            try
            {
                var realPath = Path.Combine(Environment.ContentRootPath, botFilePath);
                botConfig = BotConfiguration.Load(realPath, secretKey);
            }
            catch (Exception ex)
            {
                const string msg = @"Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.
                    - You can find the botFilePath and botFileSecret in the Azure App Service application settings.
                    - If you are running this bot locally, consider adding a appsettings.json file with botFilePath and botFileSecret.
                    - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.";

                _logger.LogCritical(msg, ex);
                throw new InvalidOperationException(msg);
            }

            services.AddSingleton(_ => botConfig);
            
            services.AddBot<LicBot>(options =>
            {
                options.CredentialProvider = new MultiCredentialProvider(botConfig);
            });
        }
    }
}
