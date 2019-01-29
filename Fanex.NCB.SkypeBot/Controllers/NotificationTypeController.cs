using System.Linq;
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
            var data = _notificationTypeService.GetAll();
            var result = new {
                data = data.ToList(),//data.Skip((pageIndex  - 1)* pageSize).Take(pageSize).ToList(),
                count = 1000 //data.Count
            };

            return Json(result);
        }

        [HttpPost]
        public IActionResult Delete(int id){
            return Json(true);
        }

        [HttpPost]
        public IActionResult Add(object param){
            return Json(true);
        }

        [HttpPost]
        public IActionResult Edit(object param){
            return Json(true);
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