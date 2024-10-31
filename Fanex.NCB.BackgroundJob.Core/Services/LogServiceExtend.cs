namespace Fanex.NCB.AutoJobs.Core.Services
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Logging.Extensions;
    using Fanex.NCB.AutoJobs.Core.Services.Interfaces;
    using Microsoft.Extensions.Logging;

    #pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplication")]
    #pragma warning restore IDE0079 // Remove unnecessary suppression
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