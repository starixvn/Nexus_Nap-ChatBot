using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class ChatGroupMappingController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {            
            return View("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }    
    }
}