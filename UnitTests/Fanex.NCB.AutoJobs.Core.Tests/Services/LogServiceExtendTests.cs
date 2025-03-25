namespace Fanex.NCB.AutoJobs.Core.Tests.Services;

using System.Threading.Tasks;
using System;
using AutoFixture;
using Logging.Extensions;
using Fanex.NCB.AutoJobs.Core.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class LogServiceExtendTests
{
    private readonly IFixture fixture = new Fixture();
    private ILogger<LogServiceExtend> filelogger;

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
        filelogger.Received().LogError(logMessage);
    }

    [Fact]
    public async Task WriteExceptionSentryErrorAsync_Exception_ExpectedWriteLogToFile()
    {
        // Arrage
        var exception = fixture.Create<Exception>();
        var service = GetService();

        // Act
        await service.WriteExceptionSentryErrorAsync(exception);

        // Assert
        filelogger.LogError(exception.ToString());
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

    [Fact]
    public async Task WriteMessageSentryErrorAsync_WhenCalled_ExpectedWriteLogToSentry()
    {
        // Arrange
        var message = fixture.Create<string>();
        var service = GetService();

        // Act
        await service.WriteMessageSentryErrorAsync(message);

        // Assert
        filelogger.Received().LogError(message);
    }

    #region Helpers

    public LogServiceExtend GetService()
    {
        filelogger = Substitute.For<ILogger<LogServiceExtend>>();
        return new LogServiceExtend(filelogger);
    }

    #endregion Helpers
}