using Fanex.NCB.SkypeBot.Model.Criteria.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class NotificationTypeController : Controller
    {
        private readonly INotificationTypeService _notificationTypeService;

        public NotificationTypeController(INotificationTypeService notificationTypeService)
        {
            _notificationTypeService = notificationTypeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/NotificationType/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageIndex, int pageSize)
        {
            var data = await _notificationTypeService.GetNotificationTypeListAsync(pageIndex, pageSize).ConfigureAwait(false);

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int notificationTypeId)
        {
            var result = await _notificationTypeService.DeleteNotificationTypeAsync(notificationTypeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateNotificationTypeCriteria item)
        {
            var result = await _notificationTypeService.CreateNotificationTypeAsync(item).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CreateNotificationTypeCriteria item)
        {
            var result = _notificationTypeService.UpdateNotificationTypeAsync(item);

            return Json(await result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var getRecipent = _notificationTypeService.GetRecipientTypesAsync();
            var getNotification = _notificationTypeService.GetNotificationSystemsAsync();

            return Json(new { recipientTypes = await getRecipent, notificationSystems = await getNotification });
        }
    }
}