using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models.ParameterObject;
using Fanex.NAP.Client.Services;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Core.Models.DataDomain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fanex.NCB.Main.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService userService;
        private readonly INapSession napSession;
        private readonly INapSettings napSettings;
        private readonly ISecurityService securityService;

        private readonly AppSettings _options;

        public AuthController(
            IUserService userService,
            INapSession napSession,
            ISecurityService securityService,
            INapSettings napSettings,
            IOptions<AppSettings> options)
        {
            this.userService = userService;
            this.napSession = napSession;
            this.napSettings = napSettings;
            this.securityService = securityService;
            _options = options.Value;
        }

        public IActionResult Index(string returnUrl, bool invalidLogin)
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.invalidLogin = invalidLogin;
            return View(new ApplicationUser());
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

            if (user != null && password.Equals(user.Password))
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
                        napSettings.Environment, true).ToList();

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));

                    return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "NewNotification"));
                }
            }

            return Json(new
            {
                IsOk = false,
                Msg = "Username or password incorrected"
            });
        }
    }
}