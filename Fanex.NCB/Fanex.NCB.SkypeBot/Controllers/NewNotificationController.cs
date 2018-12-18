using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class NewNotificationController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {            
            return View("~/Views/SkypeBot/NewNotification/Index.cshtml");
        }    
    }
}