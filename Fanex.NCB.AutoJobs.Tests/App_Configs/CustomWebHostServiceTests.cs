namespace Fanex.NCB.AutoJobs.Tests.App_Configs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Fanex.NCB.AutoJobs.Constants;
    using Fanex.NCB.Core.Models;
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
        private IOptionsMonitor<List<HangfireJobSetting>> jobSettings;
        private IRecurringJobManager recurringJob;
        private IWebHost host;

        [Fact]
        public void OnStarted_WhenCalled_WriteLogAndInitJobs()
        {
            SetupService("OnStarted");

            logger.ReceivedCalls();
            recurringJob.Received(5);
        }

        private void SetupService(string methodName)
        {
            logger = Substitute.For<ILogger<CustomWebHostService>>();
            jobSettings = Substitute.For<IOptionsMonitor<List<HangfireJobSetting>>>();

            var settings = new List<HangfireJobSetting>
            {
                new HangfireJobSetting {JobName = HangfireJob.SendNotification, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.SyncNapUserControlData, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.SyncOICData, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.PullTicketFromSpecialDeleteAndScanNoti, CronExpression = "* * * * *", IsEnabled = true},
                new HangfireJobSetting {JobName = HangfireJob.MonitorSkypebot, CronExpression = "* * * * *", IsEnabled = true}
            };
            jobSettings.CurrentValue.Returns(settings);
            recurringJob = Substitute.For<IRecurringJobManager>();
            host = Substitute.For<IWebHost>();
            host.Services.GetService<ILogger<CustomWebHostService>>().Returns(logger);
            host.Services.GetService<IOptionsMonitor<List<HangfireJobSetting>>>().Returns(jobSettings);
            host.Services.GetService<IRecurringJobManager>().Returns(recurringJob);
            var type = typeof(CustomWebHostService);
            var instance = Activator.CreateInstance(type, host);
            MethodInfo method = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(x => x.Name == methodName && x.IsVirtual);
            method.Invoke(instance, null);
        }
    }
}