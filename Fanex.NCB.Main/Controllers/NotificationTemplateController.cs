namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Main.ViewModel.NotificationTemplate;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class NotificationTemplateController : Controller
    {
        private readonly INotificationTemplaceService notificationTemplaceService;
        private readonly INapSession napSession;

        public NotificationTemplateController(INotificationTemplaceService notificationTemplaceService, INapSession napSession)
        {
            this.notificationTemplaceService = notificationTemplaceService;
            this.napSession = napSession;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateNotificationTemplateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await notificationTemplaceService.CreateAsync(new CreateNotificationTemplate
                {
                    NotificationTypeId = viewModel.NotificationTypeId,
                    NotificationTemplateName = viewModel.NotificationTemplateName,
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    CreatorId = napSession.UserId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}