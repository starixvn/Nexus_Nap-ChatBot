namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var context = HttpContext.Features.Get<IHttpConnectionFeature>();
            var ip = context?.LocalIpAddress;
            ViewBag.IpServer = ip;

            return View();
        }
    }
}