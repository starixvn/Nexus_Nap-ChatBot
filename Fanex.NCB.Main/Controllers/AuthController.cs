using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Services;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Core.Models.DataDomain;
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
        private readonly IUserService userService;
        private readonly AppSettings _options;

        public AuthController(
            IUserService userService,
            IOptions<AppSettings> options)
        {
            this.userService = userService;
            _options = options.Value;
        }

        public IActionResult Index(string returnUrl, bool invalidLogin)
        {
            ViewBag.returnUrl = returnUrl;
            ViewBag.invalidLogin = invalidLogin;
            return View(new ApplicationUser());
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            var userData = JsonConvert.DeserializeObject<ApplicationUser>(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value);
            var menuItems = await userService.GetMenuItemAsync(userData.UserId, _options.SiteId, _options.Env).ConfigureAwait(false);
            var menus = GetMenus(menuItems);
            return Json(menus);
        }

        private List<Menu> GetMenus(IEnumerable<MenuItem> menuItems)
        {
            var rootMenus = menuItems.Where(m => m.Parent == null).OrderBy(m => m.DisplayOrder);

            return BuildMenus(rootMenus, menuItems);
        }

        private List<Menu> GetSubMenus(IEnumerable<MenuItem> menuItems, int parentId)
        {
            var childMenus = menuItems.Where(m => m.Parent != null && m.Parent.Id == parentId).OrderBy(m => m.DisplayOrder);

            return BuildMenus(childMenus, menuItems);
        }

        private List<Menu> BuildMenus(IEnumerable<MenuItem> selectedMenuItems, IEnumerable<MenuItem> menuItems)
        {
            return selectedMenuItems
                .Select(m => new Menu
                {
                    Id = m.Id,
                    Name = m.Name,
                    Url = m.Url,
                    Policy = m.Policy,
                    SubMenus = GetSubMenus(menuItems, m.Id)
                })
                .ToList();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string userName, string password, string returnUrl)
        {
            return await AuthenticateUser(userName, password, returnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction("Index");
        }

        private async Task<IActionResult> AuthenticateUser(string userName, string password, string returnUrl)
        {
            var user = await userService.GetAsync(userName).ConfigureAwait(false);

            if (user != null && password.Equals(user.Password))
            {
                var result = new ApplicationUser
                {
                    FullName = string.Join(" ", user.FirstName, user.LastName),
                    UserId = user.Id,
                    UserName = user.Username,
                };
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.UserData,JsonConvert.SerializeObject(result)),
                    new Claim(ClaimTypes.Name, result.FullName)
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

                return LocalRedirect(!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "NewNotification"));
            }

            return Json(new
            {
                IsOk = false,
                Msg = "Username or password incorrected"
            });
        }
    }
}