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
        public IActionResult GetAll(){
            var result = _notificationTypeService.GetAll();

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
    }
}