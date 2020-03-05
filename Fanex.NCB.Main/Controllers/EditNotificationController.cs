namespace Fanex.NCB.Main.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public IActionResult Index(int id)
        {
            return View("~/Views/SkypeBot/EditNotification/Index.cshtml", id);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Get(int id)
        {
            var notification = await notificationService.GetEditNotificationInfoAsync(id).ConfigureAwait(false);

            if (notification != null)
            {
                return Json(notification);
            }

            return NotFound();
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> GetRecipients(int notificationId, int notificationTypeId, int? productId)
        {
            var result = await notificationService.GetEditNotificationRecipientsAsync(notificationId, notificationTypeId, productId).ConfigureAwait(false);

            var tree = GenerateTreeHelper.FromRecipient(result);

            return Json(tree);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> Edit([FromBody]EditNotificationViewModel data)
        {
            var result = new CommandResult();

            if (string.IsNullOrEmpty(data.RecipientIds))
            {
                result.Code = -1;
                result.Message = "Invaliad data. The recipients cannot be null";
            }
            else
            {
                result = await notificationService.EditAsync(new EditNotification
                {
                    NotificationId = data.NotificationId,
                    Title = data.Title,
                    Content = data.Content,
                    RecipientIds = data.RecipientIds,
                    SendDate = data.SendDate,
                    SendImmediately = data.SendImmediately,
                    UserId = napSession.UserId
                }).ConfigureAwait(false);
            }

            return Json(result);
        }
    }
}