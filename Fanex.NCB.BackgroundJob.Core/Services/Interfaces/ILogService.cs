namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    public interface ILogService
    {
        Task WriteExceptionSentryErrorAsync(Exception exception);

        Task WriteMessageSentryErrorAsync(string logMessage);

        void WriteLogFileError(string logMessage);

        void WriteLogFileWarning(string logMessage);

        void WriteLogFileInfo(string logMessage);
    }
}