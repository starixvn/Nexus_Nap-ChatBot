using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
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

        //public async Task<IActionResult> SSO()
        //{
        //    return null;
        //}

        [HttpPost]
        public async Task<IActionResult> SignIn(string userName, string password, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await AuthenticateUser(userName, password).ConfigureAwait(false);

                if (user == null)
                {
                    return RedirectToAction("Index", new {
                        returnUrl = returnUrl,
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

                    RedirectUri = "~/Home"

                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties).ConfigureAwait(false);

                return LocalRedirect(returnUrl ?? "~/Home");
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