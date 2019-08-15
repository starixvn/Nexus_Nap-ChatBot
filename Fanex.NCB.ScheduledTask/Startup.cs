namespace Fanex.NCB.ScheduledTask
{
    using System.Linq;
    using Fanex.Data;
    using Fanex.NCB.Services;
    using Fanex.NCB.Services.Interfaces;
    using Hangfire;
    using Hangfire.Dashboard;
    using Hangfire.MySql.Core;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                };
            };

            services.AddSingleton(Configuration);
            RegisterService(services);
            RegisterHangfire(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHangfireDashboard(options: new DashboardOptions
            {
                Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>()
            }).UseHangfireServer(options: new BackgroundJobServerOptions
            {
                WorkerCount = Configuration.GetValue<int>("WorkerCount")
            });

            RunHangfireJobs(Configuration);

            app.UseStaticFiles();
            app.UseMvc();
        }

        private void RegisterService(IServiceCollection services)
        {
            services.AddSingleton<ISendNotificationService, SendNotificationService>();
        }

        private void RegisterHangfire(IServiceCollection services)
        {
            var connectionString = Cryptography.DecryptConnectionString(Configuration.GetConnectionString("Hangfire"));
            services.AddHangfire(x => x.UseStorage(new MySqlStorage(connectionString)));
        }

        private static void RunHangfireJobs(IConfiguration configuration)
        {
            var cronJob = new CronJob();
            configuration.Bind("CronJob", cronJob);

            RecurringJob.AddOrUpdate<ISendNotificationService>("AutoScanAndSendNotification",
                sendNoti => sendNoti.ScanAndSendNotification(),
                cronJob.SendNotification);
        }
    }
}