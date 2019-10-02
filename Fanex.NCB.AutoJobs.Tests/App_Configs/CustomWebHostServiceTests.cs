namespace Fanex.NCB.AutoJobs.Tests.App_Configs
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Hangfire;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class CustomWebHostServiceTests
    {
        private ILogger logger;
        private IOptionsMonitor<CronJob> cronJob;
        private IRecurringJobManager recurringJob;
        private IWebHost host;

        [Fact]
        public void OnStarted_WhenCalled_WriteLogAndInitJobs()
        {
            SetupService("OnStarted");

            logger.ReceivedCalls();
            recurringJob.Received(3);
        }

        private void SetupService(string methodName)
        {
            logger = Substitute.For<ILogger<CustomWebHostService>>();
            cronJob = Substitute.For<IOptionsMonitor<CronJob>>();
            var cron = new CronJob
            {
                SendNotification = "* * * * *",
                SyncNapUserControlData = "* * * * *",
                SyncOICData = "* * * * *"
            };
            cronJob.CurrentValue.Returns(cron);
            recurringJob = Substitute.For<IRecurringJobManager>();
            host = Substitute.For<IWebHost>();
            host.Services.GetService<ILogger<CustomWebHostService>>().Returns(logger);
            host.Services.GetService<IOptionsMonitor<CronJob>>().Returns(cronJob);
            host.Services.GetService<IRecurringJobManager>().Returns(recurringJob);
            var type = typeof(CustomWebHostService);
            var instance = Activator.CreateInstance(type, host);
            MethodInfo method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(x => x.Name == methodName && x.IsVirtual);
            method.Invoke(instance, null);
        }
    }
}