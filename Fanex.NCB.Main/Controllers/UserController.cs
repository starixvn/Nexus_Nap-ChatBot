namespace Fanex.NCB.Main.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.NAPResolver.Attributes;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UserController : Controller
    {
        private readonly ISiteService siteService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;

        public UserController(
            ISecurityService securityService,
            ISiteService siteService,
            INapSession napSession,
            INapSettings napSettings)
        {
            this.securityService = securityService;
            this.siteService = siteService;
            this.napSession = napSession;
            this.napSettings = napSettings;
        }

        [HttpPost]
        public async Task<ActionResult> CheckUserOnlineStatus()
        {
            var recheckInStatus = await RecheckIn();
            return Json(new
            {
                napSession.OnUserId,
                ErrCode = (int)recheckInStatus
            });
        }

        private async Task<RecheckInStatus> RecheckIn()
        {
            if (napSession.OnUserId <= 0)
            {
                return RecheckInStatus.KickOut;
            }
            else
            {
                if (!napSettings.EnableReCheckIn)
                {
                    return RecheckInStatus.Success;
                }

                if (!await siteService.IsActivatedAsync(napSettings.SiteId))
                {
                    return RecheckInStatus.InvalidSite;
                }

                var status = await securityService.ReCheckInAsync(napSession.OnUserId, napSettings.SiteId);

                return status > 0 ? RecheckInStatus.KickOut : RecheckInStatus.Success;
            }
        }

        [HttpGet]
        public IActionResult GetMenu()
        {
            var menuItems = napSession.MenuItemList ?? new List<MenuItem>();
            var menu = GetMenus(menuItems);
            return Json(menu);
        }

        private static IEnumerable<Menu> GetMenus(IEnumerable<MenuItem> menuItems)
        {
            return menuItems.Where(m => m.Parent == null)
                .Select(m => new Menu
                {
                    Id = m.Id,
                    Name = m.Name,
                    Url = m.Url,
                    Policy = m.Policy,
                    SubMenus = menuItems.Where(sm => sm.Parent != null && sm.Parent.Id == m.Id).Select(sm => new Menu
                    {
                        Id = sm.Id,
                        Name = sm.Name,
                        Url = sm.Url,
                        Policy = sm.Policy
                    })
                });
        }
    }
}