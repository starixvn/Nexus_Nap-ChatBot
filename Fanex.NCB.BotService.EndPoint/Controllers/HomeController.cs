namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var ip = HttpContext.Connection.LocalIpAddress;
            var domain = HttpContext.Request.Host.Value;
            ViewBag.IpServer = ip.ToString();
            ViewBag.Domain = domain;

            return View("Index");
        }
    }
}