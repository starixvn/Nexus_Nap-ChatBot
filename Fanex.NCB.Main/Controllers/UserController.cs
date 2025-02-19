namespace Fanex.NCB.Main.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NAP.Client.Configuration;
    using NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Services;
    using Core.Enum;
    using Fanex.NCB.Core.Models;
    using NAPResolver.Attributes;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class UserController : Controller
    {
        private readonly ISiteService siteService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;
        private readonly bool EnableCheckPermissionChanged;

        public UserController(
            ISecurityService securityService,
            ISiteService siteService,
            INapSession napSession,
            INapSettings napSettings,
            IConfiguration configuration)
        {
            this.securityService = securityService;
            this.siteService = siteService;
            this.napSession = napSession;
            this.napSettings = napSettings;
            EnableCheckPermissionChanged = configuration.GetValue<bool>("EnableCheckPermissionChanged");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<ActionResult> CheckUserOnlineStatus()
        {
            var recheckInStatus = await RecheckIn();
            return Json(new
            {
                napSession.OnUserId,
                ErrCode = (int)recheckInStatus,
                IsChangePermission = recheckInStatus == RecheckInStatus.KickOutPermission
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
                if (!napSettings.EnableReCheckIn && !EnableCheckPermissionChanged) return RecheckInStatus.Success;

                var status = await securityService.ReCheckInAsync(napSession.OnUserId, napSettings.SiteId);
                var reCheckInStatus = (RecheckInStatus)status;

                if (!napSettings.EnableReCheckIn && reCheckInStatus == RecheckInStatus.KickOut)
                    return RecheckInStatus.Success;

                if (!EnableCheckPermissionChanged && reCheckInStatus == RecheckInStatus.KickOutPermission)
                    return RecheckInStatus.Success;

                if (!await siteService.IsActivatedAsync(napSettings.SiteId)) return RecheckInStatus.InvalidSite;

                return reCheckInStatus;
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
            return menuItems.Where(m => m.Parent == null || m.Parent.Id == 0)
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