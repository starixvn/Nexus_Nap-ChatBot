namespace Fanex.NCB.Shared.Tests.Services
{
    using Fanex.NCB.Shared.Services;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class CommonLogServiceTests
    {
        private ILogger logger;
        private NLog.Logger nlog;

        [Fact]
        public async Task SentryErrorAsync_WithException_WillCallsFanexLogError()
        {
            // Arrange
            var exception = new Exception("This is message");
            var service = GetService();

            // Act
            await service.SentryErrorAsync("message", exception);

            // Assert
            logger.Received().LogError(exception, Arg.Is<string>(x => x.Equals("message")));
        }

        [Fact]
        public void SentryError_WithException_WillCallsFanexLogError()
        {
            // Arrange
            var exception = new Exception("This is message");
            var service = GetService();

            // Act
            service.SentryError("message", exception);

            // Assert
            logger.Received().LogError(exception, Arg.Is<string>(x => x.Equals("message")));
        }

        [Fact]
        public async Task SentryInfoAsync_WithException_WillCallsFanexLogInfo()
        {
            // Arrange
            var errMsg = "This is info message";
            var service = GetService();

            // Act
            await service.SentryInfoAsync(errMsg);

            // Assert
            logger.Received().LogInformation(Arg.Is<string>(x => x.Equals(errMsg)));
        }

        [Fact]
        public void LogFileError_WithException_WillCallsNLog()
        {
            // Arrange
            var exception = new Exception("This is message");

            var service = GetService();

            // Act
            service.LogFileError(exception);

            // Assert
            nlog.Received().Error(exception);
        }

        [Fact]
        public void LogFileMessage_WithException_WillCallsNLog()
        {
            // Arrange
            var errMsg = "This is info message";

            var service = GetService();

            // Act
            service.LogFileMessage(errMsg);

            // Assert
            nlog.Received().Error(Arg.Is(errMsg));
        }

        private CommonLogService GetService()
        {

            var loggerFactory = Substitute.For<ILoggerFactory>();
            logger = Substitute.For<ILogger>();
            loggerFactory.CreateLogger<CommonLogService>().Returns(logger);
            nlog = Substitute.For<NLog.Logger>();
            return new CommonLogService(loggerFactory);
        }
    }
}