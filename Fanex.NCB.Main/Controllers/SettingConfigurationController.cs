namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.NotificationTemplate;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using System.Threading.Tasks;
    using static Fanex.NAP.Core.Business.Constant;

    //[NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SettingConfigurationController : Controller
    {
        private readonly INapSession _napSession;

        public SettingConfigurationController(INapSession napSession)
        {
            _napSession = napSession;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Read)]
        public IActionResult Index([FromServices]IUserService userService)
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(_napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.SettingConfiguration, Policy.Host);

            return View("~/Views/SkypeBot/SettingConfiguration/Index.cshtml");
        }
    }
}