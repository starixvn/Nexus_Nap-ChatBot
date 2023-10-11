namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.Hubs;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class NewNotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;
        private readonly IHubContext<ChatbotHub, IChatbotClient> chatHubContext;

        public NewNotificationController(INotificationService notificationService, INapSession napSession, IHubContext<ChatbotHub, IChatbotClient> chatHubContext)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
            this.chatHubContext = chatHubContext;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NewNotification/Index.cshtml");
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> Create(CreateNewNotificationViewModel model)
        {
            var result = new CommandResult();
            var notificationIds = string.Empty;
            if (string.IsNullOrEmpty(model.RecipientIds))
            {
                result.Code = -1;
                result.Message = "Invaliad data. The recipients cannot be null";
            }
            else
            {
                var contents = model.ChannelIds.Split(",").Select(x => byte.Parse(x))
                       .Select(x => new NotificationTemplateContent
                       {
                           ChannelId = x,
                           Content = model.Content
                       }).ToList();
                (result, notificationIds) = await notificationService.CreateAsync(new CreateNotification
                {
                    NotificationTypeId = model.NotificationTypeId,
                    ProductId = model.ProductId,
                    RecipientIds = model.RecipientIds,
                    Title = model.Title,
                    Content = JsonConvert.SerializeObject(contents),
                    SendDate = model.SendDate,
                    SendImmediately = model.SendImmediately,
                    CreatorId = napSession.UserId,
                    Platforms = model.Platforms ?? string.Empty,
                    IntegrationMethodIds = model.IntegrationMethodIds ?? string.Empty
                });

                if (result.IsSuccessful && !string.IsNullOrWhiteSpace(notificationIds))
                {
                    PushSignalRNewNotification(notificationIds);
                }
            }

            return Json(result);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetRecipients(
            int notificationTypeId, string platforms, string recipientIds, int? productId = null,
            string integrationMethodIds = null, string channelIds = null
        )
        {
            platforms = platforms ?? string.Empty;
            var (recipients, selectedRecipients) = await notificationService.GetRecipientsAsync(notificationTypeId, platforms, recipientIds, productId, integrationMethodIds, channelIds);
            var tree = GenerateTreeHelper.FromRecipient(recipients);

            return Json(new { recipients = tree, selectedRecipients });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetTemplates(int notificationTypeId)
        {
            var result = await notificationService.GetTemplatesAsync(notificationTypeId);

            return Json(result);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetResendNotification(int notificationId, string recipientIds, bool isResendAll)
        {
            ResendNotification resendNotification = await notificationService.GetResendNotificationInfoAsync(notificationId, isResendAll);
            var channelIds = resendNotification.ChannelId.ToString();
            var platformsLst = resendNotification.Platforms.Select(item => item.PlatformId).ToList();
            var platforms = string.Join(",", platformsLst);
            var integrationLst = resendNotification.IntegrationMethods.Select(item => item.IntegrationMethodId).ToList();
            var integrationIds = string.Join(",", integrationLst);
            var notiTypeId = resendNotification.NotificationTypeId;
            var (recipients, selectedRecipients) = await notificationService.GetRecipientsAsync(notiTypeId, platforms, recipientIds, integrationMethodIds: integrationIds, channelIds: channelIds);
            IEnumerable<Recipient> unSelectableRecipients = resendNotification.Recipients.Where(r => !recipients.Any(x => x.RecipientId == r.RecipientId)).ToList();

            resendNotification.Recipients = resendNotification.Recipients.Where(x => !unSelectableRecipients.Any(u => u.RecipientId == x.RecipientId)).ToList();

            var tree = GenerateTreeHelper.FromRecipient(recipients);

            return Json(new { resendNotification, recipients = tree, unSelectableRecipients, selectedRecipients });
        }

        private void PushSignalRNewNotification(string notificationIds)
        {
            Task.Run(() => chatHubContext.Clients.All.InsertNewNotifications(notificationIds));
        }
    }
}