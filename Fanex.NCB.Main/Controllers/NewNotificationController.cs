using Fanex.NAP.Client.AspNetCore.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [NapSuperAuth]
    public class NewNotificationController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NewNotification/Index.cshtml");
        }
    }
}