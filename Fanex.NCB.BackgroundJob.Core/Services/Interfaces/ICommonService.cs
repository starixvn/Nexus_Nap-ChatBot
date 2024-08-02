namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.AutoJobs.Core.Models.Common;

    public interface ICommonService
    {
        Task<string> GetJsonDataFromDbACCAsync(string tableName);

        Task<ServiceSetting> GetServiceSetting(string inServiceName);

        string GetCategogyName(string categoryId, string languageCode);

        string GetCustomerClassName(string customerClass, string languageCode);

        Task DeleteOldLog(string currentDate);
    }
}