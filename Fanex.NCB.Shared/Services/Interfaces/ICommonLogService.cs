namespace Fanex.NCB.Shared.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface ICommonLogService
    {
        void SentryError(string errMsg, Exception exception = null);

        void SentryInfo(string info);

        Task SentryErrorAsync(string errMsg, Exception exception = null);

        Task SentryInfoAsync(string info);

        void LogFileMessage(string message);

        void LogFileError(Exception exception);
    }
}