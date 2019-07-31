using System.Threading.Tasks;
using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
using Fanex.NCB.Models.ChatGroupMapping;
using Fanex.NCB.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class ChatGroupMappingController : Controller
    {
        private readonly IChatGroupMappingService chatGroupMappingService;
        private readonly ISettingService settingService;

        public ChatGroupMappingController(
            IChatGroupMappingService chatGroupMappingService,
            ISettingService settingService)
        {
            this.chatGroupMappingService = chatGroupMappingService;
            this.settingService = settingService;
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
        public async Task<IActionResult> Edit(EditChatGroupMapping model)
        {
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