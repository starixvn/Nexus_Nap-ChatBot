namespace Fanex.NCB.Main.Controllers
{
    using System.Threading.Tasks;
    using Core.Constants;
    using NAPResolver.Attributes;
    using Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ViewLogController : Controller
    {
        private readonly IViewLogService viewLogService;

        public ViewLogController(IViewLogService viewLogService)
        {
            this.viewLogService = viewLogService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/ViewLog/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public async Task<IActionResult> GetNotificationSystemMessage(int? notificationId, int pageIndex, int pageSize)
        {
            var (totalItems, systemMessages) = await viewLogService.GetNotificationSystemMessageAsync(notificationId, pageIndex, pageSize);
            return Json(new { total = totalItems, data = systemMessages });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public async Task<IActionResult> GetNotificationRecipientList(int? notificationId, int pageIndex, int pageSize)
        {
            var (totalItems, recipients) = await viewLogService.GetNotificationRecipientListAsync(notificationId, pageIndex, pageSize);

            return Json(new { total = totalItems, data = recipients });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ViewLog, Policy = Policy.Read)]
        public async Task<IActionResult> GetConversationMessageLog(int? notificationId, int pageIndex, int pageSize)
        {
            var (totalItems, conversationMessages, mapRecipient) = await viewLogService.GetConversationMessageLogAsync(notificationId, pageIndex, pageSize);

            return Json(new { total = totalItems, data = conversationMessages, dataMapRecipient = mapRecipient });
        }
    }
}