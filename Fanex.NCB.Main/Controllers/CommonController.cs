using Fanex.NAP.Client.AspNetCore.Attributes;
using Fanex.NCB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [NapSuperAuth]
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
    }
}