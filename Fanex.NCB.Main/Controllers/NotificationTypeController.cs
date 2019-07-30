using System.Threading.Tasks;
using Fanex.NCB.Models.NotificationType;
using Fanex.NCB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class NotificationTypeController : Controller
    {
        private readonly INotificationTypeService notificationTypeService;

        public NotificationTypeController(INotificationTypeService notificationTypeService)
        {
            this.notificationTypeService = notificationTypeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NotificationType/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageIndex, int pageSize)
        {
            var data = await notificationTypeService.GetAllAsync(pageIndex, pageSize).ConfigureAwait(false);

            return Json(new { NotificationTypes = data.Item1, Count = data.Item2 });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int notificationTypeId)
        {
            var result = await notificationTypeService.DeleteAsync(notificationTypeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] NotificationType item)
        {
            var result = await notificationTypeService.InsertAsync(item).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] NotificationType item)
        {
            var result = notificationTypeService.UpdateAsync(item);

            return Json(await result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var getRecipent = await notificationTypeService.GetRecipientType().ConfigureAwait(false);
            var getNotification = await notificationTypeService.GetSystem().ConfigureAwait(false);

            return Json(new { recipientTypes = getRecipent, notificationSystems = getNotification });
        }
    }
}