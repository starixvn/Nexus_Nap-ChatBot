namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Threading.Tasks;
    using NSubstitute;
    using Xunit;

    public class LogServiceExtendTests
    {
        private readonly Logging.ILogger logger;
        private readonly LogServiceExtend service;

        public LogServiceExtendTests()
        {
            logger = Substitute.For<Logging.ILogger>();
            service = new LogServiceExtend(logger);
        }

        [Fact]
        public async Task WriteExceptionSentryErrorAsync_WhenCall_ExecuteFanexLogAndNLog()
        {
            string errMsg = "Error message";
            var exception = new Exception(errMsg);
            await service.WriteExceptionSentryErrorAsync(exception);
            await logger.Received().ErrorAsync(Arg.Is<string>(err => err.Contains(errMsg)));
        }

        [Fact]
        public async Task WriteMessageSentryErrorAsync_WhenCall_WriteLogSentry()
        {
            string errMsg = "Error message";
            await service.WriteMessageSentryErrorAsync(errMsg);
            await logger.Received().ErrorAsync(errMsg);
        }
    }
}