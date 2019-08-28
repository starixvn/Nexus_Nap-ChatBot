namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Common;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Core.Resources;
    using Fanex.NCB.Models.Auth;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AuthController : Controller
    {
        private const int SSOExpiredTimeInSeconds = 30;
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;

        public AuthController(
            IUserService userService,
            IGroupService groupService,
            ISecurityService securityService,
            INapSession napSession,
            INapSettings napSettings)
        {
            this.userService = userService;
            this.groupService = groupService;
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
        public async Task<IActionResult> SignIn(UserLogin userLogin)
        {
            string errorMsg = string.Empty;
            User user = await userService.GetAsync(userLogin.Username).ConfigureAwait(false);

            if (user != null)
            {
                if (!ValidatePassword(user, userLogin))
                {
                    errorMsg = Labels.WrongPassword;
                }
                else if (!IsExistNapSite(user))
                {
                    errorMsg = Labels.NoPermision;
                    
                }
                else if (!IsExistNapGroup(user))
                {
                    errorMsg = Labels.NoPermision;
                }
                else
                {
                    await DoCheckIn(user);

                    return Redirect(!string.IsNullOrWhiteSpace(userLogin.ReturnUrl) && Url.IsLocalUrl(userLogin.ReturnUrl) ? userLogin.ReturnUrl :
                        Url.Content($"~/{napSession.MenuItemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Url))?.Url}"));
                }
            }
            else
            {
                errorMsg = Labels.UserNotExists;
            }

            return RedirectToAction("Index", new
            {
                userLogin.ReturnUrl,
                invalidLogin = true,
                errorMsg
            });
        }

        public async Task<IActionResult> Logout()
        {
            securityService.Logout(napSession.OnUserId);
            napSession.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction("Index");
        }
        
        private async Task DoCheckIn(User user)
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
        }

        [Route("userlogin/sso")]
        public async Task<IActionResult> SSO(SSOLogin ssoLogin)
        {
            if (ValidateTime(ssoLogin.Time))
            {
                var userId = int.Parse(Data.Cryptography.DecryptPassword(ssoLogin.UserId));
                var onuserid = int.Parse(Data.Cryptography.DecryptPassword(ssoLogin.OnUserId));

                var ssoLoginParam = new SSOLoginParameter
                {
                    UserId = userId,
                    SiteId = napSettings.SiteId,
                    CheckOnUserId = userId,
                    OnUserId = onuserid,
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
                    if (user != null)
                    {
                        await DoCheckIn(user).ConfigureAwait(false);

                        return Redirect(!string.IsNullOrWhiteSpace(ssoLogin.RedirectUrl) && Url.IsLocalUrl(ssoLogin.RedirectUrl) ? ssoLogin.RedirectUrl :
                        Url.Content($"~/{napSession.MenuItemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Url))?.Url}"));
                    }
                }
            }

            return NotFound();
        }

        private static bool ValidatePassword(User user, UserLogin userLogin)
        {
            return string.Equals(user.Password, Cryptography.CfsEnCode(userLogin.Password), StringComparison.CurrentCulture);
        }

        private bool IsExistNapSite(User user)
        {
            return userService
                .GetSiteLevel(user.Id, napSettings.Environment)
                .Any(s => s.Site.Id == napSettings.SiteId && s.IsActive);
        }

        private bool IsExistNapGroup(User user)
        {
            if (user.Group != null)
            {
                var group = groupService.Get(user.Group.Id);

                if (group != null && !group.IsActive)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateTime(string time)
        {
            var cfsDescryptTime = Data.Cryptography.DecryptPassword(time);
            var expiration = DateTime.Parse(securityService.GetTime()) - DateTime.Parse(cfsDescryptTime);

            return expiration.TotalSeconds <= SSOExpiredTimeInSeconds;
        }
    }
}