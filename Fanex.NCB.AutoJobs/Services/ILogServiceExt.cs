namespace Fanex.NCB.AutoJobs.Services
{
    using System;
    using System.Threading.Tasks;

    public interface ILogServiceExt
    {
        Task WriteError(Exception exception);

        void WriteInfo(string logMessage);
    }
}