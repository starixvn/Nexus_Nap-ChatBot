namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.AspNetCore.Attributes;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapSuperAuth]
    public class ChatGroupMappingController : Controller
    {
        private readonly IChatGroupMappingService chatGroupMappingService;
        private readonly ISettingService settingService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public ChatGroupMappingController(
            IChatGroupMappingService chatGroupMappingService,
            ISettingService settingService,
            INapSettings napSettings,
            INapSession napSession,
            IUserService userService)
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
        [NapAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(GetChapGroupMapping model)
        {
            var result = await chatGroupMappingService.GetChatGroupMappingListAsync(model).ConfigureAwait(false);
            var isHostPermission = userService.IsHavingPermission(napSession.UserId, napSettings.SiteId, FunctionKey.ChatGroupMapping, Policy.Host);
            return Json(new { data = result.Item1, count = result.Item2, isHost = isHostPermission });
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
    }
}