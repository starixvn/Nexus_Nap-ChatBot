namespace Fanex.NCB.AutoJobs.Core.Services
{
    using Fanex.Logging.Extensions;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Microsoft.Extensions.Logging;

    public class LogServiceExtend : ILogService
    {
        private readonly Logging.ILogger logger;
        private readonly ILogger<LogServiceExtend> filelogger;

        public LogServiceExtend(
            Logging.ILogger logger,
            ILogger<LogServiceExtend> filelogger)
        {
            this.logger = logger;
            this.filelogger = filelogger;
        }

        public async Task WriteExceptionSentryErrorAsync(Exception exception)
        {
            try
            {
                var logMessage = new LogMessage(exception.Message)
                       .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                       .ToString();
                await logger.ErrorAsync(logMessage);
            }
            catch (Exception ex)
            {
                filelogger.LogError(ex.ToString());
            }
        }

        public async Task WriteMessageSentryErrorAsync(string logMessage)
        {
            await logger.ErrorAsync(logMessage);
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
}