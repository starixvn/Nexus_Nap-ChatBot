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
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class NotificationTypeController : Controller
    {
        private readonly INotificationTypeService notificationTypeService;
        private readonly ISettingService settingService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public NotificationTypeController(INotificationTypeService notificationTypeService,
            ISettingService settingService,
            INapSession napSession,
            INapSettings napSettings,
            IUserService userService)
        {
            this.notificationTypeService = notificationTypeService;
            this.settingService = settingService;
            this.napSession = napSession;
            this.napSettings = napSettings;
            this.userService = userService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Read)]
        public async Task<IActionResult> Index()
        {
            var permission = await userService.GetPermissionAsync(napSession.UserId, napSettings.SiteId, FunctionKey.NotificationType);
            var isHost = permission >= (int)Policy.Host;
            return View("~/Views/SkypeBot/NotificationType/Index.cshtml", isHost);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(int pageIndex, int pageSize)
        {
            var data = await notificationTypeService.GetAllAsync(pageIndex, pageSize);
            return Json(new { NotificationTypes = data.Item1, Count = data.Item2 });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await notificationTypeService.DeleteAsync(id, napSession.UserId);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Host)]
        public async Task<IActionResult> Add([FromBody] NotificationTypeViewModel model)
        {
            model.UserId = napSession.UserId;
            var result = await notificationTypeService.InsertAsync(model);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Host)]
        public async Task<IActionResult> Edit([FromBody] NotificationTypeViewModel model)
        {
            model.UserId = napSession.UserId;
            var result = notificationTypeService.UpdateAsync(model);

            return Json(await result);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationType, Policy = Policy.Read)]
        public async Task<IActionResult> GetSettings()
        {
            var getRecipent = await settingService.GetRecipientTypesAsync();
            var getNotification = await settingService.GetSystemAsync();
            var getChatGroupTypes = await settingService.GetChatGroupTypeAsync();

            return Json(new { recipientTypes = getRecipent, notificationSystems = getNotification, chatGroupTypes = getChatGroupTypes });
        }
    }
}