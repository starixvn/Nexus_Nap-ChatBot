namespace Fanex.NCB.AutoJobs.Services.Implements
{
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class LogServiceExt : ILogServiceExt
    {
        private readonly Logging.ILogger logger;
        private readonly ILogger infoLogger;

        public LogServiceExt(
            Logging.ILogger logger
            , ILogger<LogServiceExt> infoLogger)
        {
            this.logger = logger;
            this.infoLogger = infoLogger;
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
             infoLogger.LogError($"{DateTime.Now}: {logMessage}");
        }
    }
}