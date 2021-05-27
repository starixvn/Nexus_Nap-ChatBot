namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
        [NapChatBotAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Read)]
        public IActionResult Index()
        {
            return View("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var viewModel = new ChatGroupMappingFilterViewModel
            {
                Bots = await settingService.GetBotNameAsync(),
                RecipientTypes = await settingService.GetFilterRecipientTypesAsync(),
                Recipients = await settingService.GetRecipientsAsync(),
                ChatGroupTypes = await settingService.GetChatGroupTypeAsync(),
                LicenseeStatuses = await settingService.GetLicenseeStatusAsync(),
                MappingStatuses = settingService.GetAllMappingStatus()
            };

            return Json(viewModel);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Read)]
        public async Task<IActionResult> GetAll(GetChapGroupMapping model)
        {
            var result = await chatGroupMappingService.GetChatGroupMappingListAsync(model);
            var permission = await userService.GetPermissionAsync(napSession.UserId, napSettings.SiteId, FunctionKey.ChatGroupMapping);
            return Json(new { data = result.Item1, count = result.Item2, isHost = permission >= (int)Policy.Host });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapChatBotAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Host)]
        public async Task<IActionResult> Edit([FromBody]EditChatGroupMapping model)
        {
            model.CreatorID = napSession.UserId;
            var result = await chatGroupMappingService.EditAsync(model);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchRecipient(string query)
        {
            return Json(await settingService.SearchRecipientsAsync(query));
        }

        [HttpGet]
        public async Task<IActionResult> SearchChatGroup(string query)
        {
            return Json(await settingService.SearchChatGroupAsync(query));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NapChatBotAuth(FunctionKey = FunctionKey.ChatGroupMapping, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateActive([FromBody]UpdateActiveConversationGroup model)
        {
            model.UserID = napSession.UserId;
            return Json(await chatGroupMappingService.UpdateActiveAsync(model));
        }
    }
}