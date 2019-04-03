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
        public async Task<IActionResult> GetSettingsAsync()
        {
            var botNames = await _settingService.GetBotNameAsync().ConfigureAwait(false);
            var recipientTypes = await _settingService.GetRecipientTypesAsync().ConfigureAwait(false);
            var mappingStatuses = _mappingStatusService.GetMappingStatus();
            var recipients = await _settingService.GetRecipientsAsync().ConfigureAwait(false);
            var chatGroupTypes = await _settingService.GetChatGroupTypeAsync();

            return Json(new { botNames, recipientTypes, mappingStatuses, recipients, chatGroupTypes });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(GetChapGroupMappingCriteria criteria)
        {
            var data = await _chatGroupMappingService.GetChatGroupMappingListAsync(criteria).ConfigureAwait(false);

            return Json(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync([FromBody] EditChatGroupMappingCriteria criteria)
        {
            var result = await _chatGroupMappingService.UpdateChatGroupMappingAsync(criteria).ConfigureAwait(false);

            return Json(result);
        }
    }
}