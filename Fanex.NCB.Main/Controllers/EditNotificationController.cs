namespace Fanex.NCB.Main.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.Hubs;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class EditNotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;
        private readonly IHubContext<ChatbotHub, IChatbotClient> chatHubContext;

        public EditNotificationController(INotificationService notificationService,
            INapSession napSession,
            IHubContext<ChatbotHub, IChatbotClient> chatHubContext)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
            this.chatHubContext = chatHubContext;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public IActionResult Index(int id)
        {
            return View("~/Views/SkypeBot/EditNotification/Index.cshtml", id);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Get(int id)
        {
            var notification = await notificationService.GetEditNotificationInfoAsync(id);
            if (notification != null)
            {
                return Json(notification);
            }

            return NotFound();
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> GetRecipients(int notificationId, int notificationTypeId, int? productId, string platforms, string recipientIds, string integrationMethodIds)
        {
            var result = await notificationService.GetEditNotificationRecipientsAsync(notificationId, notificationTypeId, productId, platforms, recipientIds, integrationMethodIds);
            var (recipients, selectedRecipients) = result;
            var tree = GenerateTreeHelper.FromRecipient(recipients);

            return Json(new { recipients = tree, selectedRecipients });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Edit(EditNotificationViewModel data)
        {
            var result = new CommandResult();

            if (string.IsNullOrEmpty(data.RecipientIds))
            {
                result.Code = -1;
                result.Message = "Invaliad data. The recipients cannot be null";
            }
            else
            {
                try
                {
                    result = await notificationService.EditAsync(new EditNotification
                    {
                        NotificationId = data.NotificationId,
                        Title = data.Title,
                        Content = data.Content,
                        RecipientIds = data.RecipientIds,
                        SendDate = data.SendDate,
                        SendImmediately = data.SendImmediately,
                        UserId = napSession.UserId,
                        Platforms = data.Platforms ?? string.Empty,
                        InIntegrationMethodIds = data.IntegrationMethodIds
                    });
                }
                catch (Exception ex)
                {
                    result.Code = 1;
                    result.Message = ex.Message;
                }

                if (result.IsSuccessful)
                {
                    PushSignalRUpdateNotificationStatus(data.NotificationId);
                }
            }

            return Json(result);
        }

        private void PushSignalRUpdateNotificationStatus(int notificationId)
        {
            Task.Run(() => chatHubContext.Clients.All.UpdateNotificationStatus(notificationId.ToString()));
        }
    }
}