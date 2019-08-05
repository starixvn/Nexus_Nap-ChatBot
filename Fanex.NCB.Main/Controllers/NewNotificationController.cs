using Fanex.NAP.Client.AspNetCore.Attributes;
using Fanex.NCB.Core.Constants;
using Microsoft.AspNetCore.Mvc;
using static Fanex.NAP.Core.Business.Constant;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [NapSuperAuth]
    public class NewNotificationController : Controller
    {
        [NapAuth(FunctionKey = FunctionKey.NewNotification, Policy = Policy.Host)]
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NewNotification/Index.cshtml");
        }
    }
}