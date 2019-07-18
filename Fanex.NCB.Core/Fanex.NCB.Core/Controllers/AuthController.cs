using Fanex.NCB.Common.DTO;
using Fanex.NCB.Common.MetalModels;
using Fanex.NCB.Core.Models.DataDomain;
using Fanex.NCB.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fanex.NCB.Main.Controllers
{
    public class AuthController : Controller
    {
        private readonly IBridgeClientService _bridgeClient;
        private readonly AppSettings _options;

        public AuthController(IBridgeClientService bridgeClient, IOptions<AppSettings> options)
        {
            _bridgeClient = bridgeClient;
            _options = options.Value;
        }

        public async Task<IActionResult> Index(string returnUrl, bool invalidLogin)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            ViewBag.returnUrl = returnUrl;
            ViewBag.invalidLogin = invalidLogin;
            return View(new ApplicationUser());
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            var userData = JsonConvert.DeserializeObject<ApplicationUser>(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value);
            var apiUrl = $"{_options.UserResourceApi}/Query/{userData.UserId}?siteid={_options.SiteId}&env={_options.Env}";
            var response = await _bridgeClient.GetJsonAsync(apiUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return Json(await response.Content.ReadAsAsync<List<Menu>>().ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string userName, string password, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await AuthenticateUser(userName, password);

                if (user == null)
                {
                    return RedirectToAction("Index", new
                    {
                        returnUrl,
                        invalidLogin = true
                    });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.UserData,JsonConvert.SerializeObject(user))
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IsPersistent = true,
                    IssuedUtc = DateTimeOffset.UtcNow,
                    RedirectUri = Url.Action("index", "Home")
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("index", "NewNotification"));
            }

            return Json(new
            {
                IsOk = false,
                Msg = "Something failed"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction("Index");
        }

        private async Task<ApplicationUser> AuthenticateUser(string userName, string password)
        {
            var user = new AuthenticationUser { UserName = userName, Password = password };
            var response = await _bridgeClient.PostJsonAsync<AuthenticationUser>(
            _options.AuthApi, user).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return await response.Content.ReadAsAsync<ApplicationUser>().ConfigureAwait(false);
        }
    }
}