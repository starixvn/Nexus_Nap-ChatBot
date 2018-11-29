using System.Diagnostics;
using Fanex.NCB.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.Main.Controllers
{
    [Authorize]
    public class SkypeBotController : Controller
    {

        public IActionResult Index()
        {
            ViewData["Message"] = HttpContext.Session.GetString("name");

            return View();
        }

    }
}
