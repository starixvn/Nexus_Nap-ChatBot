namespace Fanex.NCB.Services
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.Services.Interfaces;
    using NLog;
    using NLog.Web;

    public class LogServiceExtend : ILogService
    {
        private readonly Logging.ILogger logger;
        private readonly Logger infoLogger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public LogServiceExtend(
            Logging.ILogger logger)
        {
            this.logger = logger;
        }

        public async Task WriteExceptionSentryErrorAsync(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                       .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                       .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                       .ToString();
            await logger.ErrorAsync(logMessage);
            infoLogger.Error(logMessage);
        }

        public async Task WriteMessageSentryErrorAsync(string logMessage)
        {
            await logger.ErrorAsync(logMessage);
        }

        public void WriteLogFileInfo(string logMessage)
        {
            infoLogger.Error(logMessage);
        }
    }
}