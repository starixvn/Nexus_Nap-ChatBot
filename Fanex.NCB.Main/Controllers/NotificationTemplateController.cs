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
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class NotificationTemplateController : Controller
    {
        private readonly INotificationTemplaceService notificationTemplaceService;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public NotificationTemplateController(INotificationTemplaceService notificationTemplaceService, INapSession napSession, IUserService userService)
        {
            this.notificationTemplaceService = notificationTemplaceService;
            this.napSession = napSession;
            this.userService = userService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Read)]
        public IActionResult Index()
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.NotificationTemplate, Policy.Host);

            return View("~/Views/SkypeBot/NotificationTemplate/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Read)]
        public async Task<IActionResult> Get(int? notificationTypeId, int pageIndex, int pageSize)
        {
            var result = await notificationTemplaceService.GetAsync(notificationTypeId, pageIndex, pageSize);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Read)]
        public async Task<IActionResult> GetById(int id)
        {
            NotificationTemplate template = await notificationTemplaceService.GetByIdAsync(id);
            return Json(template);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateNotificationTemplateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult result = await notificationTemplaceService.CreateAsync(new CreateNotificationTemplate
                {
                    NotificationTypeId = viewModel.NotificationTypeId,
                    NotificationTemplateName = viewModel.NotificationTemplateName,
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    CreatorId = napSession.UserId
                });

                return Json(result);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.FirstOrDefault().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Host)]
        public async Task<IActionResult> Edit([FromBody] EditNotificationTemplateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult result = await notificationTemplaceService.EditAsync(new EditNotificationTemplate
                {
                    NotificationTemplateId = viewModel.NotificationTemplateId,
                    NotificationTypeId = viewModel.NotificationTypeId,
                    NotificationTemplateName = viewModel.NotificationTemplateName,
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    ModifierId = napSession.UserId
                });

                return Json(result);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.FirstOrDefault().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.NotificationTemplate, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int id)
        {
            CommandResult result = await notificationTemplaceService.DeleteAsync(id, napSession.UserId);

            return Json(result);
        }
    }
}