namespace Fanex.NCB.Services;

using System;
using System.Threading.Tasks;
using Logging.Extensions;
using Interfaces;
using Microsoft.Extensions.Logging;

public class LogServiceExtend : ILogService
{
    private readonly ILogger<LogServiceExtend> filelogger;

    public LogServiceExtend(
        ILogger<LogServiceExtend> filelogger)
    {
        this.filelogger = filelogger;
    }

    public Task WriteExceptionSentryErrorAsync(Exception exception)
    {
        var logMessage = new LogMessage(exception.Message)
            .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
            .ToString();
        filelogger.LogError(logMessage);
        return Task.CompletedTask;
    }

    public Task WriteMessageSentryErrorAsync(string logMessage)
    {
        filelogger.LogError(logMessage);
        return Task.CompletedTask;
    }

    public void WriteLogFileError(string logMessage)
    {
        filelogger.LogError(logMessage);
    }

    public void WriteLogFileWarning(string logMessage)
    {
        filelogger.LogWarning(logMessage);
    }

    public void WriteLogFileInfo(string logMessage)
    {
        filelogger.LogInformation(logMessage);
    }
}