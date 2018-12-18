using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class NotificationTypeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {            
            return View("~/Views/SkypeBot/NotificationType/Index.cshtml");
        }    
    }
}