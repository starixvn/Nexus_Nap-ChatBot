namespace Fanex.NCB.BotService.EndPoint
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging.Sentry;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.Shared.Middlewares;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using Sentry;

    public class Startup
    {
        private readonly ILogger logger;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Environment = env;
            this.logger = logger;
        }

        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            Dictionary<string, ConnectionConfiguration> connections = Configuration
                .GetSection("ConnectionStrings")
                .GetChildren()
                .ToDictionary(connection => connection.Key,
                              connection => new ConnectionConfiguration(connection.Key, connection.Value));

            DbSettingProviderManager
               .StartNewSession()
               .Use(connections)
               .WithMySql(resourcePath: "~/App_Data")
               .Run();

            Logging.LogManager
            .SetDefaultLogCategory(Configuration.GetSection("LogCategory").Value)
            .Use(new List<Logging.ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(Configuration.GetSection("SentryEngineOptions:Dsn").Value),
                    FrontEndDsn = new Dsn(Configuration.GetSection("SentryEngineOptions:FrontEndDsn").Value)
                })
            });

            services.AddHealthChecks().AddCheck<HealthCheckExtension>("Skype service URL");

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
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

            BotServiceDI.Init(services, Configuration, Environment, appSettings, logger);
        }

        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseHsts();
            }
            var healthCheckOptions = new HealthCheckOptions
            {
                ResponseWriter = async (c, r) =>
                {
                    var result = JsonConvert.SerializeObject(
                       new
                       {
                           checks = r.Entries.Select(e =>
                      new
                      {
                          description = e.Key,
                          status = e.Value.Status.ToString(),
                          responseTime = e.Value.Duration.TotalMilliseconds
                      }),
                           totalResponseTime = r.TotalDuration.TotalMilliseconds
                       });
                    await c.Response.WriteAsync(result);
                }
            };

            app.UseHealthChecks("/health", healthCheckOptions);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseBotFramework();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}