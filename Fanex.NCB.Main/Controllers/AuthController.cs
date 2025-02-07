namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Models.Auth;
    using Fanex.NCB.NAPResolver;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

#pragma warning disable S4487, S125

    public class AuthController : Controller
    {
        private const int SSOExpiredTimeInSeconds = 30;
        private const int DiffTimeZoneBetweenNAPServerAndChatBotServer = -4;
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;
        private readonly ISyncDataService syncDataService;

        public AuthController(
            IUserService userService,
            IGroupService groupService,
            ISecurityService securityService,
            INapSession napSession,
            INapSettings napSettings,
            ISyncDataService syncDataService)
        {
            this.userService = userService;
            this.groupService = groupService;
            this.securityService = securityService;
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

        [HttpPost]
        public async Task<IActionResult> SignIn(UserLogin userLogin)
        {
            string errorMsg = string.Empty;
            User user = await userService.GetAsync(userLogin.Username);

            if (user != null)
            {
                if (!ValidatePassword(user, userLogin))
                {
                    errorMsg = Resources.Resources.WrongPassword;
                }
                else if (!await IsExistNapSite(user) || !await IsExistNapGroup(user))
                {
                    errorMsg = Resources.Resources.NoPermision;
                }
                else
                {
                    var onUserId = await LoginCheckIn(user);
                    await DoCheckIn(user, onUserId);

                    var url = napSession.MenuItemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Url))?.Url;
                    var urlContent = Url.Content($"~{(!string.IsNullOrEmpty(url) ? url : "/Error/UnauthorizedUser")}");

                    return Redirect(!string.IsNullOrWhiteSpace(userLogin.ReturnUrl) && Url.IsLocalUrl(userLogin.ReturnUrl) ? $"~{userLogin.ReturnUrl}" : urlContent);
                }
            }
            else
            {
                errorMsg = Resources.Resources.UserNotExists;
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }

        [Route("userlogin/sso")]
        public async Task<IActionResult> SSO(SsoLogin ssoLogin)
        {
            var isValidTime = await IsValidateTime(ssoLogin.Time);
            if (isValidTime)
            {
                var userId = int.Parse(CryptographyExtend.DecryptPassword(ssoLogin.UserId.Replace(" ", "+")));
                var onUserid = int.Parse(CryptographyExtend.DecryptPassword(ssoLogin.OnUserId.Replace(" ", "+")));

                var newOnUserId = await SSOLogin(userId, onUserid);

                if (newOnUserId > 0)
                {
                    var user = await userService.GetAsync(userId);
                    if (user != null)
                    {
                        await DoCheckIn(user, newOnUserId);

                        var url = napSession.MenuItemList.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Url))?.Url;
                        var urlContent = Url.Content($"~{(!string.IsNullOrEmpty(url) ? url : "/Error/UnauthorizedUser")}");

                        return Redirect(!string.IsNullOrWhiteSpace(ssoLogin.RedirectUrl) && Url.IsLocalUrl(ssoLogin.RedirectUrl) ? ssoLogin.RedirectUrl : urlContent);
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
                Domain = HttpContext.Request?.Host.ToString(),
                CheckOnUserId = onuserId,
                IP = GetClientIP(HttpContext),
                BrowserName = Request?.Headers["User-Agent"].ToString(),
                CountryCode = ResolveCountry(HttpContext.Request)?.GeoId.ToString(CultureInfo.CurrentCulture),
                NetSessionId = HttpContext?.Session.Id,
                Platform = Environment.OSVersion.Platform.ToString(),
                LoginDate = DateTime.Now
            };
            return await securityService.SSOLoginAsync(ssoLoginParam);
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
                Domain = HttpContext.Request?.Host.ToString(),
                Ip = GetClientIP(HttpContext),
                BrowserName = Request?.Headers["User-Agent"].ToString(),
                CountryCode = ResolveCountry(HttpContext.Request)?.GeoId.ToString(CultureInfo.CurrentCulture),
                NetSessionId = HttpContext.Session.Id,
                Platform = Environment.OSVersion.Platform.ToString(),
                LoginDate = DateTime.Now
            };
            return await securityService.LoginAsync(loginParam);
        }

        private static string GetClientIP(HttpContext httpContext)
        {
            var clientIP = httpContext.Request?.Headers["x-forwarded-for"].ToString();
            if (string.IsNullOrWhiteSpace(clientIP))
            {
                clientIP = httpContext?.Connection?.RemoteIpAddress?.ToString();
            }

            return clientIP;
        }

        private static RegionInfo ResolveCountry(HttpRequest request)
        {
            var culture = ResolveCulture(request);

            if (culture != null)
            {
                return new RegionInfo(culture.LCID);
            }

            return null;
        }

        private static CultureInfo ResolveCulture(HttpRequest request)
        {
            var userLangs = request.Headers["Accept-Language"].ToString();
            var firstLang = userLangs.Split(',').FirstOrDefault();
            var defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
            return CultureInfo.CreateSpecificCulture(defaultLang);
        }

        private static bool ValidatePassword(User user, UserLogin userLogin)
        {
            return string.Equals(user.Password, userLogin.Password, StringComparison.CurrentCulture);
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
            var cfsDescryptTime = CryptographyExtend.DecryptPassword(time.Replace(" ", "+"));
            var serverTime = await securityService.GetTimeAsync();
            var serverDateTime = NapServicesRegistrationExtension.NAPResolverType == NapResolverType.Local
                ? DateTime.Parse(serverTime, new CultureInfo("en-US")).AddHours(DiffTimeZoneBetweenNAPServerAndChatBotServer)
                : DateTime.Parse(serverTime, new CultureInfo("en-US"));
            var expiration = serverDateTime - DateTime.Parse(cfsDescryptTime, new CultureInfo("en-US"));

            return expiration.TotalSeconds <= SSOExpiredTimeInSeconds;
        }
    }

#pragma warning restore S4487, S125
}