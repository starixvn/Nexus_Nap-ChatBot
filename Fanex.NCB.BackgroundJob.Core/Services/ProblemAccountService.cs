namespace Fanex.NCB.AutoJobs.Core.Services;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants;
using Criteria.ProblemAccount;
using Models.Common;
using Fanex.NCB.AutoJobs.Core.Models.Core;
using Models.NotifictaionSendMessage;
using Models.ProblemAccounts;
using Interfaces;
using Newtonsoft.Json;

public class ProblemAccountService : IProblemAccountService
{
    private readonly IBaseService baseService;
    private readonly ICommonService commonService;
    private readonly IChatbotService chatbotService;

    public ProblemAccountService(
        IBaseService baseService,
        ICommonService commonService,
        IChatbotService chatbotService
    )
    {
        this.baseService = baseService;
        this.commonService = commonService;
        this.chatbotService = chatbotService;
    }

    public async Task SendMessageProblemAccountAsync()
    {
        var messageSetting = await GetMessageSettingAsync();
        if (messageSetting != null)
        {
            var (message, categoriesId, customerClass) = await GetProblemAccountAsync(messageSetting);

            if (message.Any() && categoriesId.Any() && customerClass.Any())
            {
                var (command, notifications) =
                    await ImportMessageFromProblemAccountAsync(message, categoriesId, customerClass);
                await commonService.OnAfterImportDataAsync(command, notifications);
            }
        }
    }

    public async Task<MessageSetting> GetMessageSettingAsync()
    {
        var criteria = new GetMessageSettingCriteria();
        return await baseService.GetDataEntityAsync<MessageSetting>(criteria);
    }

    public async Task<(IEnumerable<ProblemAccountInfo>, IEnumerable<short>, IEnumerable<short>)> GetProblemAccountAsync(
        MessageSetting messageSetting)
    {
        var criteria = new GetProblemAccountInfoCriteria
        {
            BatchSize = messageSetting.BatchSize,
            LastLogId = messageSetting.LastLogId
        };
        return await baseService.FetchMultipleAsync<ProblemAccountInfo, short, short>(criteria);
    }

    public IEnumerable<Category> GetAllCategory(IEnumerable<short> categoriesId)
    {
        var categories = new List<Category>();
        foreach (int categoryId in categoriesId)
        {
            var categoryNameEN = commonService.GetCategogyName(categoryId.ToString(), Language.en_US);
            var categoryNameCS = commonService.GetCategogyName(categoryId.ToString(), Language.zh_CN);
            categories.Add(new Category
            {
                CategoryId = categoryId.ToString(),
                CategoryNameEN = categoryNameEN,
                CategoryNameCS = categoryNameCS
            });
        }

        return categories;
    }

    public IEnumerable<CustomerClassInfo> GetAllCustomerClass(IEnumerable<short> customerClass)
    {
        var customerClassInfo = new List<CustomerClassInfo>();
        foreach (int customer in customerClass)
        {
            var customerClassNameEN = commonService.GetCustomerClassName(customer.ToString(), Language.en_US);
            var customerClassNameCS = commonService.GetCustomerClassName(customer.ToString(), Language.zh_CN);
            customerClassInfo.Add(new CustomerClassInfo
            {
                CustomerClass = customer.ToString(),
                CustomerClassNameEN = customerClassNameEN,
                CustomerClassNameCS = customerClassNameCS
            });
        }

        return customerClassInfo;
    }

    public async Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMessageFromProblemAccountAsync(
        IEnumerable<ProblemAccountInfo> message,
        IEnumerable<short> categoriesId,
        IEnumerable<short> customerClass
    )
    {
        var criteria = new ImportFromProblemAccountCriteria
        {
            Message = JsonConvert.SerializeObject(message),
            CategoryInfo = JsonConvert.SerializeObject(GetAllCategory(categoriesId)),
            CustomerClassInfo = JsonConvert.SerializeObject(GetAllCustomerClass(customerClass))
        };

        var (recipients, notificationMessages) =
            await baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(criteria);
        var notifications =
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients);
        var command = new CommandResult
        {
            Code = criteria.ErrCode,
            Message = criteria.ErrMsg
        };

        return (command, notifications);
    }
}