using System.Linq;
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
        public IActionResult GetAll(int pageIndex, int pageSize){
            var data = _notificationTypeService.GetNotificationTypeList(pageIndex, pageSize);           

            return Json(data);
        }

        [HttpPost]
        public IActionResult Delete([FromBody] int notificationTypeId){
            var result = _notificationTypeService.DeleteNotificationType(notificationTypeId);

            return Json(result);
        }

        [HttpPost]
        public IActionResult Add([FromBody] NotificationTypeItem item){
            var result =_notificationTypeService.CreateNotificationType(item);

            return Json(result);
        }

        [HttpPost]
        public IActionResult Edit([FromBody] NotificationTypeItem item)
        {
            var result = _notificationTypeService.UpdateNotificationType(item);

            return Json(result);
        }

        [HttpGet]
        public IActionResult GetSettings(){
            var result = new {
                recipientTypes = _notificationTypeService.GetRecipientTypes(),
                notificationSystems = _notificationTypeService.GetNotificationSystems()
            };

            return Json(result);
        }
    }
}