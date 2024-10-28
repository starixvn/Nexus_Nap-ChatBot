namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Logging.Extensions;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Xunit;

    public class LogServiceExtendTests
    {
        private readonly IFixture fixture = new Fixture();
        private Logging.ILogger logger;
        private ILogger<LogServiceExtend> filelogger;

        #region WriteExceptionSentryErrorAsync

        [Fact]
        public async Task WriteExceptionSentryErrorAsync_WhenCalled_ExpectedWriteLogToSentry()
        {
            // Arrage
            var exception = fixture.Create<Exception>();
            var logMessage = new LogMessage(exception.Message)
                        .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                        .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                        .ToString();
            var service = GetService();

            // Act
            await service.WriteExceptionSentryErrorAsync(exception);

            // Assert
            await logger.Received().ErrorAsync(logMessage);
        }

        [Fact]
        public async Task WriteExceptionSentryErrorAsync_Exception_ExpectedWriteLogToFile()
        {
            // Arrage
            var exception = fixture.Create<Exception>();
            var service = GetService();
            logger.When(x => x.ErrorAsync(Arg.Any<string>())).Do(x => throw exception);

            // Act
            await service.WriteExceptionSentryErrorAsync(exception);

            // Assert
            filelogger.LogError(exception.ToString());
        }

        #endregion WriteExceptionSentryErrorAsync

        [Fact]
        public async Task WriteMessageSentryErrorAsync_WhenCalled_ExpectedWriteLogToSentry()
        {
            // Arrange
            var message = fixture.Create<string>();
            var service = GetService();

            // Act
            await service.WriteMessageSentryErrorAsync(message);

            // Assert
            await logger.Received().ErrorAsync(message);
        }

        [Fact]
        public void WriteLogFileError_WhenCalled_ExpectedWriteLogToFile()
        {
            // Arrange
            var message = fixture.Create<string>();
            var service = GetService();

            // Act
            service.WriteLogFileError(message);

            // Assert
            filelogger.Received().LogError(message);
        }

        [Fact]
        public void WriteLogFileWarning_WhenCalled_ExpectedWriteLogToFile()
        {
            // Arrange
            var message = fixture.Create<string>();
            var service = GetService();

            // Act
            service.WriteLogFileWarning(message);

            // Assert
            filelogger.Received().LogWarning(message);
        }

        [Fact]
        public void WriteLogFileInfo_WhenCalled_ExpectedWriteLogToFile()
        {
            // Arrange
            var message = fixture.Create<string>();
            var service = GetService();

            // Act
            service.WriteLogFileInfo(message);

            // Assert
            filelogger.Received().LogInformation(message);
        }

        #region Helpers

        public LogServiceExtend GetService()
        {
            logger = Substitute.For<Logging.ILogger>();
            filelogger = Substitute.For<ILogger<LogServiceExtend>>();
            return new LogServiceExtend(logger, filelogger);
        }

        #endregion Helpers
    }
}