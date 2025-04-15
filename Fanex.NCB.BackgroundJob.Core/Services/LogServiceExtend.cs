namespace Fanex.NCB.AutoJobs.Core.Services;

using Logging.Extensions;
using Interfaces;
using Microsoft.Extensions.Logging;

public class LogServiceExtend : ILogService
{
    private readonly ILogger<LogServiceExtend> _logger;

    public LogServiceExtend(
        ILogger<LogServiceExtend> logger)
    {
        _logger = logger;
    }

    public Task WriteExceptionSentryErrorAsync(Exception exception)
    {
        var logMessage = new LogMessage(exception.Message)
            .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
            .ToString();
        _logger.LogError(logMessage);
        return Task.CompletedTask;

    }

    public Task WriteMessageSentryErrorAsync(string logMessage)
    {
        _logger.LogError(logMessage);
        return Task.CompletedTask;
    }

    public void WriteLogFileError(string logMessage)
    {
        _logger.LogError(logMessage);
    }

    public void WriteLogFileWarning(string logMessage)
    {
        _logger.LogWarning(logMessage);
    }

    public void WriteLogFileInfo(string logMessage)
    {
        _logger.LogInformation(logMessage);
    }
}