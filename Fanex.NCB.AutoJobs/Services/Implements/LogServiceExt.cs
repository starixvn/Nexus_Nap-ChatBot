namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;

    public class LogServiceExt : ILogServiceExt
    {
        private readonly Logging.ILogger logger;

        private readonly NLog.Logger infoLogger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public LogServiceExt(
            Logging.ILogger logger)
        {
            this.logger = logger;
        }

        public async Task WriteError(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                       .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                       .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                       .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                       .ToString();
            await logger.ErrorAsync(logMessage);
        }

        public void WriteInfo(string logMessage)
        {
            infoLogger.Error(logMessage);
        }
    }
}