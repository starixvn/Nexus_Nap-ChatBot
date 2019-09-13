namespace Fanex.NCB.AutoJobs.Tests.Extensions
{
    using System.ServiceProcess;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Fanex.NCB.AutoJobs.Extensions;
    using Hangfire;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class WebHostServiceExtensionTests
    {
        private readonly IWebHost host;
        private readonly ILogger logger;
        private readonly IOptionsMonitor<CronJob> cronJob;
        private readonly IRecurringJobManager recurringJob;

        public WebHostServiceExtensionTests()
        {
            host = Substitute.For<IWebHost>();
            logger = Substitute.For<ILogger<CustomWebHostService>>();
            cronJob = Substitute.For<IOptionsMonitor<CronJob>>();
            recurringJob = Substitute.For<IRecurringJobManager>();

            host.Services.GetService<ILogger<CustomWebHostService>>().Returns(logger);
            host.Services.GetService<IOptionsMonitor<CronJob>>().Returns(cronJob);
            host.Services.GetService<IRecurringJobManager>().Returns(recurringJob);
        }

        [Fact]
        public void RunAsCustomServie_WhenIsService_BaseServiceWillBeCalled()
        {
            var fakeService = new CustomWebHostService(host);
            //Actual
            WebHostServiceExtension.RunAsCustomService(host, true);

            //Assert
            recurringJob.Received(3).ReceivedCalls();
            ServiceBase.Run(fakeService);
        }
    }
}