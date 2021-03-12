namespace Fanex.NCB.Main.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        public IActionResult PageNotFound()
        {
            return View();
        }

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