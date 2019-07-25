using System.Threading.Tasks;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using Fanex.NCB.SkypeBot.Services.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var filterRecipientTypeTask = _settingService.GetFilterRecipientTypesAsync();
            var recipientTypeTask = _settingService.GetRecipientTypesAsync();
            var recipientTask = _settingService.GetRecipientsAsync();
            var chatGroupTypeTask = _settingService.GetChatGroupTypeAsync();
            var licenseeStatusTask = _settingService.GetLicenseeStatusAsync();

            await Task.WhenAll(botNameTask, recipientTask, chatGroupTypeTask, recipientTypeTask, licenseeStatusTask).ConfigureAwait(false);
            return Json(new
            {
                botNames = await botNameTask,
                recipientTypes = await recipientTypeTask,
                filterRecipientTypes = await filterRecipientTypeTask,
                recipients = await recipientTask,
                chatGroupTypes = await chatGroupTypeTask,
                licenseeStatuses = await licenseeStatusTask,
                mappingStatuses = _mappingStatusService.GetMappingStatus()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(GetChapGroupMappingCriteria criteria)
        {
            var model = new Criteria.ChatGroupMapping.GetChapGroupMappingCriteria
            {
                BotNames = criteria.BotNames,
                RecipientName = criteria.RecipientName,
                RecipientTypeIDs = criteria.RecipientTypeIDs,
                LicenseeStatusIds = criteria.LicenseeStatusIds,
                MappingStatusIds = criteria.MappingStatusIds,
                PageIndex = criteria.PageIndex,
                PageSize = criteria.PageSize,
            };

            var result = await _chatGroupMappingService.GetChatGroupMappingListAsync(model).ConfigureAwait(false);
            return Json(new { data = result.Item1, count = result.Item2 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] EditChatGroupMappingCriteria criteria)
        {
            return Json(null);
        }

        [HttpGet]
        public async Task<IActionResult> SearchRecipient(string query)
        {
            return Json(await _settingService.SearchRecipientsAsync(query).ConfigureAwait(false));
        }
    }
}