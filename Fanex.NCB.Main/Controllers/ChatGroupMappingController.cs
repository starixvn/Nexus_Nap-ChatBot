namespace Fanex.NCB.SkypeBot.Controllers
{
    using Fanex.NAP.Client.AspNetCore.Attributes;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;
    using static Fanex.NAP.Core.Business.Constant;

    [NapSuperAuth]
    public class ChatGroupMappingController : Controller
    {
        private readonly IChatGroupMappingService chatGroupMappingService;
        private readonly ISettingService settingService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly IUserService userService;
        private readonly ILogger logger;

        public ChatGroupMappingController(
            IChatGroupMappingService chatGroupMappingService,
            ISettingService settingService,
            INapSettings napSettings,
            INapSession napSession,
            IUserService userService,
            ILogger<ChatGroupMappingController> logger)
        {
            this.chatGroupMappingService = chatGroupMappingService;
            this.settingService = settingService;
            this.napSettings = napSettings;
            this.napSession = napSession;
            this.userService = userService;
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Read)]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var bots = settingService.GetBotNameAsync();
            var recipientTypes = settingService.GetFilterRecipientTypesAsync();
            var recipients = settingService.GetRecipientsAsync();
            var chatgroupTypes = settingService.GetChatGroupTypeAsync();
            var licenseeStatus = settingService.GetLicenseeStatusAsync();

            await Task.WhenAll(bots, recipientTypes, recipients, chatgroupTypes, licenseeStatus).ConfigureAwait(false);

            var viewModel = new ChatGroupMappingFilterViewModel
            {
                Bots = await bots,
                RecipientTypes = await recipientTypes,
                Recipients = await recipients,
                ChatGroupTypes = await chatgroupTypes,
                LicenseeStatuses = await licenseeStatus,
                MappingStatuses = settingService.GetAllMappingStatus()
            };

            return Json(viewModel);
        }

        [HttpGet]
        [NapAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(GetChapGroupMapping model)
        {
            var result = await chatGroupMappingService.GetChatGroupMappingListAsync(model).ConfigureAwait(false);
            var permission = await userService.GetPermissionAsync(napSession.UserId, napSettings.SiteId, FunctionKey.ChatGroupMapping).ConfigureAwait(false);
            return Json(new { data = result.Item1, count = result.Item2, isHost = permission >= (int)Policy.Host });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Host)]
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

        [HttpGet]
        public async Task<IActionResult> SearchChatGroup(string query)
        {
            return Json(await settingService.SearchChatGroupAsync(query).ConfigureAwait(false));
        }
    }
}