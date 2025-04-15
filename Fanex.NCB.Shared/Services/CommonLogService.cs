namespace Fanex.NCB.Shared.Services
{
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Logging;
    using NLog;
    using NLog.Web;
    using System;
    using System.Threading.Tasks;

    public class CommonLogService : ICommonLogService
    {
        private readonly Microsoft.Extensions.Logging.ILogger logger;
        private readonly NLog.Logger nlog = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        public CommonLogService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<CommonLogService>();
        }

        public void LogFileError(Exception exception)
        {
            nlog.Error(exception);
        }

        public void LogFileMessage(string message)
        {
            nlog.Error(message);
        }

        public void SentryError(string errMsg, Exception exception = null)
        {
            logger.LogError(exception, errMsg);
        }

        public Task SentryErrorAsync(string errMsg, Exception exception = null)
        {
            logger.LogError(exception, errMsg);
            return Task.CompletedTask;
        }

        public Task SentryInfoAsync(string info)
        {
            logger.LogInformation(info);
            return Task.CompletedTask;
        }
    }
}