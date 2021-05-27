namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NCB.Main.ViewModel.TreeSelect;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using Fanex.NCB.Models.Common;

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
            var result = await commonService.GetNotificationTypesAsync();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var result = await commonService.GetProductsAsync();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationStatuses()
        {
            var result = await commonService.GetNotificationStatusesAsync();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipients()
        {
            var result = await commonService.GetRecipientsAsync();
            var tree = GenerateTreeHelper.FromRecipient(result);

            return Json(tree);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await commonService.GetUsersAsync();

            return Json(result);
        }

        [HttpGet]
        public IActionResult GetSettingTypes()
        {
            var result = new List<SettingType>
            {
                new SettingType
                {
                    Type = "Msg Contents Type",
                    Value = 1
                }
            };

            return Json(result);
        }
    }
}