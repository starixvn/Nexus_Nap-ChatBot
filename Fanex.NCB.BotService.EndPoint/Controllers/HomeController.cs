namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var localIpAddress = HttpContext.Connection.LocalIpAddress;
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            ViewBag.LocalIpAddress = localIpAddress.ToString();
            ViewBag.RemoteIpAddress = remoteIpAddress.ToString();

            return View("Index");
        }
    }
}