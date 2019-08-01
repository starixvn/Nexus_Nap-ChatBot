namespace Fanex.NCB.Main.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Common;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController : Controller
    {
        private readonly IUserService userService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;

        public AuthController(
            IUserService userService,
            INapSession napSession,
            ISecurityService securityService,
            INapSettings napSettings)
        {
            this.userService = userService;
            this.napSession = napSession;
            this.napSettings = napSettings;
            this.securityService = securityService;
        }

        public IActionResult Index(string returnUrl, bool invalidLogin)
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.invalidLogin = invalidLogin;
            return View();
        }

        [HttpGet]
        public IActionResult GetMenu()
        {
            var menu = GetMenus(napSession.MenuItemList);
            return Json(menu);
        }

        private IEnumerable<Menu> GetMenus(IEnumerable<MenuItem> menuItems)
        {
            return menuItems
                .Select(m => new Menu
                {
                    Id = m.Id,
                    Name = m.Name,
                    Url = m.Url,
                    Policy = m.Policy,
                });
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string userName, string password, string returnUrl)
        {
            return await AuthenticateUser(userName, password, returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            napSession.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction("Index");
        }

        private async Task<IActionResult> AuthenticateUser(string userName, string password, string returnUrl)
        {
            var user = await userService.GetAsync(userName).ConfigureAwait(false);

            if (user != null && user.Password == Cryptography.CfsEnCode(password))
            {
                var siteLevelList = userService.GetSiteLevel(user.Id, napSettings.Environment);

                if (siteLevelList.Any(s => s.Site.Id == napSettings.SiteId))
                {
                    var loginParam = new LoginParameter
                    {
                        UserId = user.Id,
                        SiteId = napSettings.SiteId,
                        NetSessionId = HttpContext.Session.Id,
                        Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                        LoginDate = DateTime.Now
                    };

                    var onlineUserId = securityService.Login(loginParam);

                    napSession.UserName = user.Username;
                    napSession.UserId = user.Id;
                    napSession.OnUserId = onlineUserId;
                    napSession.MenuItemList = userService.GetMenuItem(user.Id, napSettings.SiteId,
                        napSettings.Environment, true).Where(x => !string.IsNullOrWhiteSpace(x.Url))
                        .OrderBy(x => x.DisplayOrder).ToList();

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));

                    return Redirect(!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl :
                        Url.Content($"~/{napSession.MenuItemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Url))?.Url}"));
                }
            }

            return RedirectToAction("Index", new
            {
                returnUrl,
                invalidLogin = true
            });
        }
    }
}