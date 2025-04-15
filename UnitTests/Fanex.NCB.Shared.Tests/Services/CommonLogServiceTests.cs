namespace Fanex.NCB.Shared.Tests.Services
{
    using Fanex.NCB.Shared.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Testing;
    using NSubstitute;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class CommonLogServiceTests
    {
        private FakeLogger<CommonLogService> logger;
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
            Assert.Equal("message", logger.LatestRecord.Message);
            Assert.Equal(exception, logger.LatestRecord.Exception);
            Assert.Equal(LogLevel.Error, logger.LatestRecord.Level);
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
            Assert.Equal("message", logger.LatestRecord.Message);
            Assert.Equal(exception, logger.LatestRecord.Exception);
            Assert.Equal(LogLevel.Error, logger.LatestRecord.Level);
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
            Assert.Equal(errMsg, logger.LatestRecord.Message);
            Assert.Equal(LogLevel.Information, logger.LatestRecord.Level);
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
            logger = new FakeLogger<CommonLogService>();
            loggerFactory.CreateLogger<CommonLogService>().Returns(logger);
            nlog = Substitute.For<NLog.Logger>();
            return new CommonLogService(loggerFactory);
        }
    }
}