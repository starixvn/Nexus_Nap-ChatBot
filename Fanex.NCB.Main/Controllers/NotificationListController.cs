namespace Fanex.NCB.SkypeBot.Controllers
{
    using Fanex.NAP.Client.AspNetCore.Attributes;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [NapSuperAuth]
    public class NotificationListController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;

        public NotificationListController(INotificationService notificationService, INapSession napSession)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NotificationList/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> Get(GetNotificationViewModel filter)
        {
            var result = await notificationService.GetNotificationsAsync(new GetNotifications
            {
                NotificationTypeIds = filter.NotificationTypeIds,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate,
                CreatorId = filter.CreatorId,
                NotificationStatusIds = filter.NotificationStatusIds,
                RecipientIds = filter.RecipientIds,
                PageIndex = filter.PageIndex,
                PageSize = filter.PageSize
            }).ConfigureAwait(false);

            return Json(new { total = result.Item1, data = result.Item2 });
        }
    }
}