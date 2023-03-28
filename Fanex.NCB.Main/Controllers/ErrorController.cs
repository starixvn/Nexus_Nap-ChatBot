namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NCB.NAPResolver.Attributes;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        public IActionResult PageNotFound()
        {
            return View();
        }

        [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult UnauthorizedUser()
        {
            return View();
        }

        public IActionResult InternalServerError()
        {
            return View();
        }
    }
}