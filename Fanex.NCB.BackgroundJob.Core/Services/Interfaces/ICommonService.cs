namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using Models.Common;
    using Fanex.NCB.AutoJobs.Core.Models.Core;
    using Models.NotifictaionSendMessage;

    public interface ICommonService
    {
        Task<string> GetJsonDataFromDbACCAsync(string tableName);

        Task<ServiceSetting> GetServiceSetting(string inServiceName);

        string GetCategogyName(string categoryId, string languageCode);

        string GetCustomerClassName(string customerClass, string languageCode);

        Task DeleteOldLog(string currentDate);

        Task SendMessageByChannelAsync(byte channelId, IEnumerable<NotificationMessage> notifications);

        Task OnAfterImportDataAsync(CommandResult commandResult, IEnumerable<NotificationMessage> notificationMessages);
    }
}