namespace Fanex.NCB.Main.Controllers
{
    using Fanex.NCB.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class WhoamiController : Controller
    {
        [Route("Whoami")]
        [Route("Whoami.aspx")]
        [AllowAnonymous]
        public IActionResult Whoami()
        {
            return Content(WhoamiHelper.GetContent(HttpContext));
        }
    }
}