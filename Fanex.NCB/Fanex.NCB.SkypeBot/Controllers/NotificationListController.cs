using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class NotificationListController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {            
            return View();
        }    
    }
}