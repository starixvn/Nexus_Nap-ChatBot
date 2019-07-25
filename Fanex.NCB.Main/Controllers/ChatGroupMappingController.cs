using System.Threading.Tasks;
using Fanex.NCB.Models.ChatGroupMapping;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.NCB.SkypeBot.Controllers
{
    [Authorize]
    public class ChatGroupMappingController : Controller
    {
        private readonly IChatGroupMappingService chatGroupMappingService;
        private readonly ISettingService settingService;
        private readonly IMappingStatusService _mappingStatusService;

        public ChatGroupMappingController(
            IChatGroupMappingService chatGroupMappingService,
            ISettingService settingService,
            IMappingStatusService mappingStatusService)
        {
            this.chatGroupMappingService = chatGroupMappingService;
            this.settingService = settingService;
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
            var botNameTask = settingService.GetBotNameAsync();
            var filterRecipientTypeTask = settingService.GetFilterRecipientTypesAsync();
            var recipientTypeTask = settingService.GetRecipientTypesAsync();
            var recipientTask = settingService.GetRecipientsAsync();
            var chatGroupTypeTask = settingService.GetChatGroupTypeAsync();
            var licenseeStatusTask = settingService.GetLicenseeStatusAsync();

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