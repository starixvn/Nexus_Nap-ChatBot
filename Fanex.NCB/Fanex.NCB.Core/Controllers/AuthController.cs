using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fanex.NCB.Main.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.Main.Controllers
{
    public class AuthController : Controller
    {
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
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
                    new Claim(ClaimTypes.Role, "Administrator"),
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