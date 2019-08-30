namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Models.NotificationType;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth]
    public class NotificationTypeController : Controller
    {
        private readonly INotificationTypeService notificationTypeService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public NotificationTypeController(INotificationTypeService notificationTypeService,
            INapSession napSession,
            INapSettings napSettings,
            IUserService userService)
        {
            this.notificationTypeService = notificationTypeService;
            this.napSession = napSession;
            this.napSettings = napSettings;
            this.userService = userService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Read)]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NotificationType/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(int pageIndex, int pageSize)
        {
            var data = await notificationTypeService.GetAllAsync(pageIndex, pageSize).ConfigureAwait(false);
            var permission = await userService.GetPermissionAsync(napSession.UserId, napSettings.SiteId, FunctionKey.ChatGroupMapping).ConfigureAwait(false);
            return Json(new { NotificationTypes = data.Item1, Count = data.Item2, IsHost = permission >= (int)Policy.Host });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Host)]
        public async Task<IActionResult> Delete([FromBody] int notificationTypeId)
        {
            var result = await notificationTypeService.DeleteAsync(notificationTypeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Host)]
        public async Task<IActionResult> Add([FromBody] NotificationType item)
        {
            var result = await notificationTypeService.InsertAsync(item).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Host)]
        public async Task<IActionResult> Edit([FromBody] NotificationType item)
        {
            var result = notificationTypeService.UpdateAsync(item);

            return Json(await result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var getRecipent = await notificationTypeService.GetRecipientType().ConfigureAwait(false);
            var getNotification = await notificationTypeService.GetSystem().ConfigureAwait(false);

            return Json(new { recipientTypes = getRecipent, notificationSystems = getNotification });
        }
    }
}