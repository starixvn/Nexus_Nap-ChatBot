using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using Fanex.NCB.SkypeBot.Services.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class ChatGroupMappingController : Controller
    {
        private readonly IChatGroupMappingService _chatGroupMappingService;
        private readonly ISettingService _settingService;
        private readonly IMappingStatusService _mappingStatusService;

        public ChatGroupMappingController(
            IChatGroupMappingService chatGroupMappingService,
            ISettingService settingService,
            IMappingStatusService mappingStatusService)
        {
            _chatGroupMappingService = chatGroupMappingService;
            _settingService = settingService;
            _mappingStatusService = mappingStatusService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var botNameTask = _settingService.GetBotNameAsync();
            var recipientTypeTask = _settingService.GetRecipientTypesAsync();
            var recipientTask = _settingService.GetRecipientsAsync();
            var chatGroupTypeTask = _settingService.GetChatGroupTypeAsync();
            var licenseeStatusTask = _settingService.GetLicenseeStatusAsync();

            await Task.WhenAll(botNameTask, recipientTask, chatGroupTypeTask, recipientTypeTask, licenseeStatusTask).ConfigureAwait(false);
            return Json(new
            {
                botNames = await botNameTask,
                recipientTypes = await recipientTypeTask,
                recipients = await recipientTask,
                chatGroupTypes = await chatGroupTypeTask,
                licenseeStatuses = await licenseeStatusTask,
                mappingStatuses = _mappingStatusService.GetMappingStatus()

            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetChapGroupMappingCriteria criteria)
        {
            return Json(await _chatGroupMappingService.GetChatGroupMappingListAsync(criteria).ConfigureAwait(false));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] EditChatGroupMappingCriteria criteria)
        {
            return Json(await _chatGroupMappingService.UpdateChatGroupMappingAsync(criteria).ConfigureAwait(false));
        }

        [HttpGet]
        public async Task<IActionResult> SearchRecipient(string query)
        {
            return Json(await _settingService.SearchRecipientsAsync(query).ConfigureAwait(false));
        }
    }
}