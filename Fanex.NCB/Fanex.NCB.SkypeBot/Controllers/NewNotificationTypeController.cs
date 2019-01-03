using Fanex.NCB.SkypeBot.Services.NotificationType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class NewNotificationTypeController : Controller
    {
        private readonly INotificationTypeService _notificationTypeService;        
        
        public NewNotificationTypeController(INotificationTypeService notificationTypeService)
        {
            _notificationTypeService = notificationTypeService;
        }

        [HttpGet]
        public IActionResult Index()
        {            
            return View("~/Views/SkypeBot/NewNotificationType/Index.cshtml");
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