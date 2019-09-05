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
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Core.Resources;
    using Fanex.NCB.Models.Auth;
    using Fanex.NCB.NAPResolver;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController : Controller
    {
        private const int SSOExpiredTimeInSeconds = 30;
        private const int DiffTimeZoneBetweenNAPServerAndChatBotServer = -4;
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly ISiteService siteService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;
        private readonly ISyncDataService syncDataService;

        public AuthController(
            IUserService userService,
            IGroupService groupService,
            ISecurityService securityService,
            ISiteService siteService,
            INapSession napSession,
            INapSettings napSettings,
            ISyncDataService syncDataService)
        {
            this.userService = userService;
            this.groupService = groupService;
            this.securityService = securityService;
            this.siteService = siteService;
            this.napSession = napSession;
            this.napSettings = napSettings;
            this.syncDataService = syncDataService;
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
                else if (!await IsExistNapSite(user) || !await IsExistNapGroup(user))
                {
                    errorMsg = Labels.NoPermision;
                }
                else
                {
                    var onUserId = await LoginCheckIn(user);
                    await DoCheckIn(user, onUserId);

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
            await securityService.LogoutAsync(napSession.OnUserId);
            napSession.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction("Index");
        }

        [Route("userlogin/sso")]
        public async Task<IActionResult> SSO(SSOLogin ssoLogin)
        {
            var isValidTime = await IsValidateTime(ssoLogin.Time);
            if (isValidTime)
            {
                var userId = int.Parse(Data.Cryptography.DecryptPassword(ssoLogin.UserId.Replace(" ", "+")));
                var onUserid = int.Parse(Data.Cryptography.DecryptPassword(ssoLogin.OnUserId.Replace(" ", "+")));

                await syncDataService.SyncNapDataForSSOLogin(userId).ConfigureAwait(false);

                var newOnUserId = await SSOLogin(userId, onUserid).ConfigureAwait(false);

                if (newOnUserId > 0)
                {
                    var user = await userService.GetAsync(userId).ConfigureAwait(false);
                    if (user != null)
                    {
                        await DoCheckIn(user, newOnUserId);

                        return Redirect(!string.IsNullOrWhiteSpace(ssoLogin.RedirectUrl) && Url.IsLocalUrl(ssoLogin.RedirectUrl) ? ssoLogin.RedirectUrl :
                        Url.Content($"~/{napSession.MenuItemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Url))?.Url}"));
                    }
                }
            }

            return new RedirectResult("~/Auth/Index");
        }

        private async Task<int> SSOLogin(int userId, int onuserId)
        {
            var ssoLoginParam = new SSOLoginParameter
            {
                UserId = userId,
                SiteId = napSettings.SiteId,
                CheckOnUserId = onuserId,
                IP = HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                BrowserName = Request.Headers["User-Agent"].ToString(),
                NetSessionId = HttpContext.Session.Id,
                Platform = Environment.OSVersion.Platform.ToString(),
                LoginDate = DateTime.Now
            };
            return await securityService.SSOLoginAsync(ssoLoginParam).ConfigureAwait(false);
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

        private async Task<RecheckInStatus> RecheckIn()
        {
            if (!napSettings.EnableReCheckIn || napSession.OnUserId <= 0)
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

        private async Task DoCheckIn(User user, int onUserId)
        {
            napSession.UserName = user.Username;
            napSession.UserId = user.Id;
            napSession.OnUserId = onUserId;
            napSession.MenuItemList = (await userService.GetMenuItemAsync(user.Id, napSettings.SiteId,
                napSettings.Environment, true)).Where(x => !string.IsNullOrWhiteSpace(x.Url))
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

        private async Task<int> LoginCheckIn(User user)
        {
            var loginParam = new LoginParameter
            {
                UserId = user.Id,
                SiteId = napSettings.SiteId,
                NetSessionId = HttpContext.Session.Id,
                Ip = HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                LoginDate = DateTime.Now
            };

            return await securityService.LoginAsync(loginParam);
        }

        private static bool ValidatePassword(User user, UserLogin userLogin)
        {
            return string.Equals(user.Password, Cryptography.CfsEnCode(userLogin.Password), StringComparison.CurrentCulture);
        }

        private async Task<bool> IsExistNapSite(User user)
        {
            return (await userService.GetSiteLevelAsync(user.Id, napSettings.Environment)).Any(s => s.Site.Id == napSettings.SiteId && s.IsActive);
        }

        private async Task<bool> IsExistNapGroup(User user)
        {
            if (user.Group != null)
            {
                var group = await groupService.GetAsync(user.Group.Id);

                if (group != null && !group.IsActive)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> IsValidateTime(string time)
        {
            var cfsDescryptTime = Data.Cryptography.DecryptPassword(time.Replace(" ", "+"));
            var serverTime = await securityService.GetTimeAsync();
            var serverDateTime = NapServicesRegistrationExtension.NAPResolverType == NAPResolverType.Local
                ? DateTime.Parse(serverTime).AddHours(DiffTimeZoneBetweenNAPServerAndChatBotServer)
                : DateTime.Parse(serverTime);
            var expiration = serverDateTime - DateTime.Parse(cfsDescryptTime);

            return expiration.TotalSeconds <= SSOExpiredTimeInSeconds;
        }
    }
}