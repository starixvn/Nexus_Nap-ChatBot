using System.Linq;
using System.Threading.Tasks;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.Services.NotificationType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAll(int pageIndex, int pageSize){
            var data = await _notificationTypeService.GetNotificationTypeListAsync(pageIndex, pageSize).ConfigureAwait(false);           

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int notificationTypeId){
            var result = await _notificationTypeService.DeleteNotificationTypeAsync(notificationTypeId).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] NotificationTypeItem item){
            var result = await _notificationTypeService.CreateNotificationTypeAsync(item).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] NotificationTypeItem item)
        {
            var result = await _notificationTypeService.UpdateNotificationTypeAsync(item).ConfigureAwait(false);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings(){
            var result = new {
                recipientTypes = await _notificationTypeService.GetRecipientTypesAsync().ConfigureAwait(false),
                notificationSystems = await _notificationTypeService.GetNotificationSystemsAsync().ConfigureAwait(false)
            };

            return Json(result);
        }
    }
}