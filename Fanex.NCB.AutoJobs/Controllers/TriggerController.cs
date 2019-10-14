namespace Fanex.NCB.AutoJobs.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class TriggerController : ControllerBase
    {
        private readonly ISendNotificationService sendNotificationService;

        public TriggerController(ISendNotificationService sendNotificationService)
        {
            this.sendNotificationService = sendNotificationService;
        }

        public IActionResult SendNotiImmediately(int notificationId)
        {
            Task.Run(() => sendNotificationService.SendNotificationImmediately(notificationId));

            return Ok();
        }
    }
}