namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class NotificationTemplateController : Controller
    {
        private readonly INotificationTemplaceService notificationTemplaceService;

        public NotificationTemplateController(INotificationTemplaceService notificationTemplaceService)
        {
            this.notificationTemplaceService = notificationTemplaceService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NotificationTemplate/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? notificationTypeId, int pageIndex, int pageSize)
        {
            var result = await notificationTemplaceService.GetAsync(notificationTypeId, pageIndex, pageSize).ConfigureAwait(false);

            return Json(new { total = result.Item1, data = result.Item2 });
        }
    }
}