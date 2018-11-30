using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Fanex.NCB.Core.Models.DataDomain;
using Fanex.NCB.Core.Services;
using Fanex.NCB.Main.Models.DTO;
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
        public AuthController(IBridgeClientService bridgeClient)
        {
            _bridgeClient = bridgeClient;
        }
        public async Task<IActionResult> Index(string returnUrl)
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.returnUrl = returnUrl;
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
                var user = await AuthenticateUser(userName, password);

                if (user == null)
                {
                    return Json(new
                    {
                        IsOk = false,
                        Msg = "Invalid login attempt."
                    });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, user.Group),
                    new Claim("Menu", JsonConvert.SerializeObject(user.Menu))// move to session

                };
                foreach (var item in user.Permissions) //move to session
                {
                    claims.Add(new Claim(item, item));
                }
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
                    authProperties);

                HttpContext.Session.SetString("name", userName);

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

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }

        private async Task<ApplicationUser> AuthenticateUser(string userName, string password)
        {
            var user = new { Username = userName, Password = password };
            var response = await _bridgeClient.PostJsonAsync(
            "http://localhost:52085/api/Auth/Verify", user);

            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return await response.Content.ReadAsAsync<ApplicationUser>();
        }
    }
}