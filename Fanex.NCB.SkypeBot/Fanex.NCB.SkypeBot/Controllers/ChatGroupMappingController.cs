using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
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

        public ChatGroupMappingController(IChatGroupMappingService chatGroupMappingService, ISettingService settingService)
        {
            _chatGroupMappingService = chatGroupMappingService;
            _settingService = settingService;
        }

        [HttpGet]
        public IActionResult Index()
        {            
            return View("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var botNames = _chatGroupMappingService.GetBotNameAsync();
            var recipientTypes = _settingService.GetRecipientTypesAsync();
            var mappingStatuses = _chatGroupMappingService.GetMappingStatus();
            var recipients = _settingService.GetRecipientsAsync();
            return Json(new { botNames = await botNames, recipientTypes = await recipientTypes, mappingStatuses = mappingStatuses, recipients = await recipients });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetChapGroupMappingCriteria criteria)
        {
            var data = await _chatGroupMappingService.GetChatGroupMappingListAsync(criteria).ConfigureAwait(false);

            return Json(data);
        }
        

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditChatGroupMappingCriteria item)
        {
            var result = _chatGroupMappingService.UpdateChatGroupMappingAsync(item);            
            return Json(await result);
        }
    }
}