namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class ChatGroupMappingController : Controller
    {
        private readonly IChatGroupMappingService chatGroupMappingService;
        private readonly ISettingService settingService;
        private readonly INapSession napSession;

        public ChatGroupMappingController(
            IChatGroupMappingService chatGroupMappingService,
            ISettingService settingService,
            INapSession napSession)
        {
            this.chatGroupMappingService = chatGroupMappingService;
            this.settingService = settingService;
            this.napSession = napSession;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var viewModel = new ChatGroupMappingFilterViewModel
            {
                Bots = await settingService.GetBotNameAsync().ConfigureAwait(false),
                RecipientTypes = await settingService.GetFilterRecipientTypesAsync().ConfigureAwait(false),
                Recipients = await settingService.GetRecipientsAsync().ConfigureAwait(false),
                ChatGroupTypes = await settingService.GetChatGroupTypeAsync().ConfigureAwait(false),
                LicenseeStatuses = await settingService.GetLicenseeStatusAsync().ConfigureAwait(false),
                MappingStatuses = settingService.GetAllMappingStatus()
            };

            return Json(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetChapGroupMapping model)
        {
            var result = await chatGroupMappingService.GetChatGroupMappingListAsync(model).ConfigureAwait(false);
            return Json(new { data = result.Item1, count = result.Item2 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody]EditChatGroupMapping model)
        {
            model.CreatorID = napSession.UserId;
            var result = await chatGroupMappingService.EditAsync(model).ConfigureAwait(false);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchRecipient(string query)
        {
            return Json(await settingService.SearchRecipientsAsync(query).ConfigureAwait(false));
        }
    }
}