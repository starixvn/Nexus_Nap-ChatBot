namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ViewLogController : Controller
    {
        private readonly IViewLogService viewLogService;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public ViewLogController(IViewLogService viewLogService, INapSession napSession, IUserService userService)
        {
            this.viewLogService = viewLogService;
            this.napSession = napSession;
            this.userService = userService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public IActionResult Index()
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.ViewLog, Policy.Host);
            return View("~/Views/SkypeBot/ViewLog/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public async Task<IActionResult> GetSystemMessage(int? notificationId, int pageIndex, int pageSize)
        {
            var result = await viewLogService.GetNotificationSystemMessage(notificationId, pageIndex, pageSize);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public async Task<IActionResult> GetRecipientList(int? notificationId, int pageIndex, int pageSize)
        {
            var result = await viewLogService.GetNotificationRecipientList(notificationId, pageIndex, pageSize);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public async Task<IActionResult> GetConversationMessage(int? notificationId, int pageIndex, int pageSize)
        {
            var result = await viewLogService.GetConversationMessageLog(notificationId, pageIndex, pageSize);

            return Json(new { total = result.Item1, data = result.Item2 , dataMapRecipient = result.Item3});
        }
    }
}
