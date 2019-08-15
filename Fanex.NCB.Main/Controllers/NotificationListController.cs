namespace Fanex.NCB.SkypeBot.Controllers
{
    using Fanex.NAP.Client.AspNetCore.Attributes;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.Notification;
    using Fanex.NCB.Models.Notification;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static Fanex.NAP.Core.Business.Constant;

    [NapSuperAuth]
    public class NotificationListController : Controller
    {
        private readonly INotificationService notificationService;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public NotificationListController(INotificationService notificationService, INapSession napSession, IUserService userService)
        {
            this.notificationService = notificationService;
            this.napSession = napSession;
            this.userService = userService;
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host)]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NotificationList/Index.cshtml");
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host)]
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

        [HttpPost]
        [NapAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host)]
        public async Task<IActionResult> Cancel(int notificationId)
        {
            var result = await notificationService.CancelAsync(notificationId, napSession.UserId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKey.NotifiactionList, Policy = Policy.Host)]
        public async Task<IActionResult> GetNotificationError(int notificationId)
        {
            var result = await notificationService.GetNotificationErrorAsync(notificationId).ConfigureAwait(false);

            if (result.ResolvedBy.HasValue)
            {
                var user = await userService.GetAsync(result.ResolvedBy.Value);

                result.ResolvedByUserName = user.Username;
            }

            return Json(result);
        }
    }
}