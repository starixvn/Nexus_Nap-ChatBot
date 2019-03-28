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
        public async Task<IActionResult> GetSettingsAsync()
        {
            var botNames = await _chatGroupMappingService.GetBotNameAsync();
            var recipientTypes = await _settingService.GetRecipientTypesAsync();
            var mappingStatuses = _chatGroupMappingService.GetMappingStatus();
            var recipients = await _settingService.GetRecipientsAsync();

            return Json(new { botNames, recipientTypes, mappingStatuses, recipients });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(GetChapGroupMappingCriteria criteria)
        {
            var data = await _chatGroupMappingService.GetChatGroupMappingListAsync(criteria);

            return Json(data);
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync([FromBody] EditChatGroupMappingCriteria criteria)
        {
            var result = await _chatGroupMappingService.UpdateChatGroupMappingAsync(criteria);            

            return Json(result);
        }
    }
}