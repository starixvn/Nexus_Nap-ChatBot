namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class EditNotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;

        public EditNotificationController(INotificationService notificationService, INapSession napSession)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            return View("~/Views/SkypeBot/EditNotification/Index.cshtml", id);
        }

        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var notification = await notificationService.GetEditNotificationInfo(id).ConfigureAwait(false);

            return Json(notification);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipients(int notificationTypeId, int? productId)
        {
            var result = await notificationService.GetRecipientsAsync(notificationTypeId, productId).ConfigureAwait(false);

            var tree = GenerateTreeHelper.FromRecipient(result);

            return Json(tree);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]EditNotificationViewModel data)
        {
            var result = await notificationService.EditAsync(new EditNotification
            {
                NotificationId = data.NotificationId,
                Title = data.Title,
                Content = data.Content,
                RecipientIds = data.RecipientIds,
                SendDate = data.SendDate,
                SendImmediately = data.SendImmediately,
                UserId = napSession.UserId
            }).ConfigureAwait(false);

            return Json(result);
        }
    }
}