using System.Threading.Tasks;
using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
using Fanex.NCB.Models.ChatGroupMapping;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
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
                BotNames = await settingService.GetBotNameAsync().ConfigureAwait(false),
                RecipientTypes = await settingService.GetRecipientTypesAsync().ConfigureAwait(false),
                FilterRecipientTypes = await settingService.GetFilterRecipientTypesAsync().ConfigureAwait(false),
                Recipients = await settingService.GetRecipientsAsync().ConfigureAwait(false),
                ChatGroupTypes = await settingService.GetChatGroupTypeAsync().ConfigureAwait(false),
                LicenseeStatuses = await settingService.GetLicenseeStatusAsync().ConfigureAwait(false),
                MappingStatuses = settingService.GetAllMappingStatus()
            };

            return Json(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetChapGroupMappingViewModel mode)
        {
            var model = new GetChapGroupMapping
            {
                BotIDs = mode.BotNames,
                RecipientName = mode.RecipientName,
                RecipientTypeIDs = mode.RecipientTypeIDs,
                LicenseeStatusIds = mode.LicenseeStatusIds,
                MappingStatusIds = mode.MappingStatusIds,
                PageIndex = mode.PageIndex,
                PageSize = mode.PageSize,
            };

            var result = await chatGroupMappingService.GetChatGroupMappingListAsync(model).ConfigureAwait(false);
            return Json(new { data = result.Item1, count = result.Item2 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] EditChatGroupMapping model)
        {
            var result = await chatGroupMappingService.Update(model).ConfigureAwait(false);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchRecipient(string query)
        {
            return Json(await settingService.SearchRecipientsAsync(query).ConfigureAwait(false));
        }
    }
}