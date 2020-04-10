namespace Fanex.NCB.Main.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    //[NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SettingConfigurationController : Controller
    {
        private readonly INapSession _napSession;
        private readonly ISettingConfigurationService _settingConfigurationService;

        public SettingConfigurationController(INapSession napSession, ISettingConfigurationService settingConfigurationService)
        {
            _napSession = napSession;
            _settingConfigurationService = settingConfigurationService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Read)]
        public IActionResult Index([FromServices]IUserService userService)
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(_napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.SettingConfiguration, Policy.Host);

            return View("~/Views/SkypeBot/SettingConfiguration/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetSettingTypes(int pageIndex, int pageSize)
        {
            var result = await _settingConfigurationService.GetSettingTypesAsync(pageIndex, pageSize).ConfigureAwait(false);

            return Json(new { total = result.Item1, data = result.Item2 });
        }
    }
}