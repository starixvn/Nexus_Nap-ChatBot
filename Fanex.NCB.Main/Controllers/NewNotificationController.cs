namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth]
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
            var result = await notificationService.CreateAsync(new CreateNotification
            {
                NotificationTypeId = model.NotificationTypeId,
                ProductId = model.ProductId,
                RecipientIds = model.RecipientIds,
                Title = model.Title,
                Content = model.Content,
                SendDate = model.SendDate,
                SendImmediately = model.SendImmediately,
                CreatorId = napSession.UserId
            }).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetRecipients(int notificationTypeId, int? productId)
        {
            var result = await notificationService.GetRecipientsAsync(notificationTypeId, productId).ConfigureAwait(false);

            var tree = GenerateTreeHelper.FromRecipient(result);

            return Json(tree);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetTemplates(int notificationTypeId)
        {
            var result = await notificationService.GetTemplatesAsync(notificationTypeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        public async Task<IActionResult> GetResendNotification(int notificationId, bool isResendAll)
        {
            var resendNotification = await notificationService.GetResendNotificationInfoAsync(notificationId, isResendAll);

            var recipients = await notificationService.GetRecipientsAsync(resendNotification.NotificationTypeId, null);
            var unSelectableRecipients = resendNotification.Recipients.Where(r => !recipients.Any(x => x.RecipientId == r.RecipientId));

            resendNotification.Recipients = resendNotification.Recipients.Where(x => !unSelectableRecipients.Any(u => u.RecipientId == x.RecipientId));

            var tree = GenerateTreeHelper.FromRecipient(recipients);

            return Json(new { resendNotification, recipients = tree, unSelectableRecipients });
        }
    }
}