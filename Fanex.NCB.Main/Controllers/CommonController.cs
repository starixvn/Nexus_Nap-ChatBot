namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CommonController : Controller
    {
        private readonly ICommonService commonService;

        public CommonController(ICommonService commonService)
        {
            this.commonService = commonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationTypes()
        {
            var result = await commonService.GetNotificationTypesAsync().ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var result = await commonService.GetProductsAsync().ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationStatuses()
        {
            var result = await commonService.GetNotificationStatusesAsync().ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipients()
        {
            var result = await commonService.GetRecipientsAsync().ConfigureAwait(false);
            var tree = GenerateTreeHelper.FromRecipient(result);

            return Json(tree);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await commonService.GetUsersAsync().ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettingTypes()
        {
            var result = await commonService.GetSettingTypesAsync().ConfigureAwait(false);
            return Json(result);
        }
    }
}