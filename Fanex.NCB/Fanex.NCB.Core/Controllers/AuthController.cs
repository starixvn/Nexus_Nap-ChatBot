using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Fanex.NCB.Main.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fanex.NCB.Main.Controllers
{
    public class AuthController : Controller
    {
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
                var client = new HttpClient();
                //  var user = await  AuthenticateUser(userName, password);
                var response = await client.PostAsJsonAsync(
                "http://localhost:52085/Auth/Verify", new { userName, password });
                response.EnsureSuccessStatusCode();

                // return URI of the created resource.
                var user = await response.Content.ReadAsAsync<ApplicationUser>();
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
                    new Claim(ClaimTypes.Role, "Administrator"),//> map to nap core user group
                    new Claim("Permission", new List<string>().ToString()) // map to nap user permission

                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. Required when setting the 
                    // ExpireTimeSpan option of CookieAuthenticationOptions 
                    // set with AddCookie. Also required when setting 
                    // ExpiresUtc.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
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

            await Task.Delay(500);

            if (userName == "alex")
            {
                return new ApplicationUser()
                {
                    UserName = userName,
                    FullName = "Alex Nguyen"
                };
            }
            else
            {
                return null;
            }
        }
    }
}