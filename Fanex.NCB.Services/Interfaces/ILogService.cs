namespace Fanex.NCB.Services.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface ILogService
    {
        Task WriteExceptionSentryErrorAsync(Exception exception);

        void WriteLogFileInfo(string logMessage);
    }
}