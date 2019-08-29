using Fanex.NAP.Client.AspNetCore.Attributes;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NCB.Core.Constants;
using Fanex.NCB.Main.ViewModel.Notification;
using Fanex.NCB.Main.ViewModel.TreeSelect;
using Fanex.NCB.Models.Notification;
using Fanex.NCB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Fanex.NAP.Core.Business.Constant;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [NapSuperAuth]
    public class NewNotificationController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;

        public NewNotificationController(INotificationService notificationService, INapSession napSession)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
        }

        [NapAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host, PermissionCache = true)]
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NewNotification/Index.cshtml");
        }

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host, PermissionCache = true)]
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
        [NapAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host, PermissionCache = true)]
        public async Task<IActionResult> GetRecipients(int notificationTypeId, int? productId)
        {
            var result = await notificationService.GetRecipientsAsync(notificationTypeId, productId).ConfigureAwait(false);

            var tree = GenerateTreeHelper.FromRecipient(result);

            return Json(tree);
        }
    }
}