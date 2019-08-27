namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Common;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.NAPResolver;
    using Fanex.NCB.NAPResolver.Model;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AuthController : Controller
    {
        private readonly IUserService userService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;

        public AuthController(
            Func<NAPResolverType, IUserService> userServiceProvider,
            Func<NAPResolverType, ISecurityService> securityServiceProvider,
            INapSession napSession,
            INapSettings napSettings,
            IOptionsSnapshot<NapOption> napOption)
        {
            userService = userServiceProvider(napOption.Value.NAPResolverType);
            this.napSession = napSession;
            this.napSettings = napSettings;
            securityService = securityServiceProvider(napOption.Value.NAPResolverType);
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
            string msg = string.Empty;
            User user = await userService.GetAsync(userName).ConfigureAwait(false);

            if (user != null && user.Password == Cryptography.CfsEnCode(password))
            {
                var siteLevelList = await userService.GetSiteLevelAsync(user.Id, napSettings.Environment).ConfigureAwait(false);
                if (siteLevelList.Any(s => s.Site.Id == napSettings.SiteId))
                {
                    return await DoCheckIn(user, returnUrl);
                }
                else
                {
                    msg = "Don't have permission on current site";
                }
            }
            else
            {
                msg = user == null ? "User does not exist" : "Wrong password";
            }

            return RedirectToAction("Index", new
            {
                returnUrl,
                invalidLogin = true,
                errorMsg = msg
            });
        }

        private async Task<IActionResult> DoCheckIn(User user, string returnUrl)
        {
            var loginParam = new LoginParameter
            {
                UserId = user.Id,
                SiteId = napSettings.SiteId,
                NetSessionId = HttpContext.Session.Id,
                Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                LoginDate = DateTime.Now
            };

            var onUserId = await securityService.LoginAsync(loginParam);

            napSession.UserName = user.Username;
            napSession.UserId = user.Id;
            napSession.OnUserId = onUserId;
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

        public async Task<IActionResult> SSO(int userId, int onUserId, string redirectUrl)
        {
            var ssoLoginParam = new SSOLoginParameter
            {
                UserId = userId,
                SiteId = napSettings.SiteId,
                CheckOnUserId = userId,
                OnUserId = onUserId,
                IP = HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                BrowserName = Request.Headers["User-Agent"].ToString(),
                NetSessionId = HttpContext.Session.Id,
                Platform = Environment.OSVersion.Platform.ToString(),
                LoginDate = DateTime.Now
            };
            var result = await securityService.SSOLoginAsync(ssoLoginParam).ConfigureAwait(false);

            if (result > 0)
            {
                var user = await userService.GetAsync(userId).ConfigureAwait(false);
                await DoCheckIn(user, redirectUrl).ConfigureAwait(false);

                return Redirect(!string.IsNullOrWhiteSpace(redirectUrl) && Url.IsLocalUrl(redirectUrl) ? redirectUrl :
                Url.Content($"~/{napSession.MenuItemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Url))?.Url}"));
            }

            return NotFound();
        }
    }
}