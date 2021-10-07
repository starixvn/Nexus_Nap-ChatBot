namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Core.Constants;
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
    using Newtonsoft.Json;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class NewNotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;

        public NewNotificationController(INotificationService notificationService, INapSession napSession)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
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
                       });

                result = await notificationService.CreateAsync(new CreateNotification
                {
                    NotificationTypeId = model.NotificationTypeId,
                    ProductId = model.ProductId,
                    RecipientIds = model.RecipientIds,
                    Title = model.Title,
                    Content = JsonConvert.SerializeObject(contents),
                    SendDate = model.SendDate,
                    SendImmediately = model.SendImmediately,
                    CreatorId = napSession.UserId
                });
            }

            return Json(result);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetRecipients(int notificationTypeId, int? productId = null, string channelIds = null)
        {
            IEnumerable<Recipient> recipients = await notificationService.GetRecipientsAsync(notificationTypeId, productId, channelIds);
            var tree = GenerateTreeHelper.FromRecipient(recipients);

            return Json(tree);
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
        public async Task<IActionResult> GetResendNotification(int notificationId, bool isResendAll)
        {
            ResendNotification resendNotification = await notificationService.GetResendNotificationInfoAsync(notificationId, isResendAll);
            var channelIds = resendNotification.ChannelId.ToString();
            IEnumerable<Recipient> recipients = await notificationService.GetRecipientsAsync(resendNotification.NotificationTypeId, channelIds: channelIds);
            IEnumerable<Recipient> unSelectableRecipients = resendNotification.Recipients.Where(r => !recipients.Any(x => x.RecipientId == r.RecipientId)).ToList();

            resendNotification.Recipients = resendNotification.Recipients.Where(x => !unSelectableRecipients.Any(u => u.RecipientId == x.RecipientId)).ToList();

            var tree = GenerateTreeHelper.FromRecipient(recipients);

            return Json(new { resendNotification, recipients = tree, unSelectableRecipients });
        }
    }
}