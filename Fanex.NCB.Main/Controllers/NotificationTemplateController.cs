namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.NotificationTemplate;
    using Fanex.NCB.Models.NotificationTemplate;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth]
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
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Read)]
        public IActionResult Index([FromServices]IUserService userService)
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotificationTemplate, Policy.Host);

            return View("~/Views/SkypeBot/NotificationTemplate/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Read)]
        public async Task<IActionResult> Get(int? notificationTypeId, int pageIndex, int pageSize)
        {
            var result = await notificationTemplaceService.GetAsync(notificationTypeId, pageIndex, pageSize).ConfigureAwait(false);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await notificationTemplaceService.GetByIdAsync(id).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Host)]
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

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Host)]
        public async Task<IActionResult> Edit([FromBody]EditNotificationTemplateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await notificationTemplaceService.EditAsync(new EditNotificationTemplate
                {
                    NotificationTemplateId = viewModel.NotificationTemplateId,
                    NotificationTypeId = viewModel.NotificationTypeId,
                    NotificationTemplateName = viewModel.NotificationTemplateName,
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    ModifierId = napSession.UserId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await notificationTemplaceService.DeleteAsync(id, napSession.UserId).ConfigureAwait(false);

            return Json(result);
        }
    }
}