namespace Fanex.NCB.SkypeBot.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel;
    using Fanex.NCB.Main.ViewModel.ChatGroupMapping;
    using Fanex.NCB.Models.ChatGroupMapping;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Models.Setting;
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
        private readonly ICommonService commonService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly IUserService userService;

        public ChatGroupMappingController(
            IChatGroupMappingService chatGroupMappingService,
            ISettingService settingService,
            ICommonService commonService,
            INapSettings napSettings,
            INapSession napSession,
            IUserService userService)
        {
            this.chatGroupMappingService = chatGroupMappingService;
            this.settingService = settingService;
            this.commonService = commonService;
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
            IEnumerable<ChannelInfo> channels = await commonService.GetConversationChannelAsync();
            IEnumerable<BotName> bots = await settingService.GetBotNameAsync();

            var viewModel = new ChatGroupMappingFilterViewModel
            {
                ChannelIds = string.Join(",", channels.Select(item => item.ChannelId)),
                BotIds = string.Join(",", bots.Select(item => item.Id)),
                Channels = channels.Select(item => new MultiSelectViewModel
                {
                    Id = item.ChannelId,
                    Text = item.ChannelName,
                    Checked = true,
                    IsLeaf = true
                }).ToList(),
                Bots = bots.GroupBy(item => new { item.ChannelId, item.ChannelName }).Select(item => new MultiSelectViewModel
                {
                    Id = -item.Key.ChannelId,
                    Text = item.Key.ChannelName,
                    Checked = true,
                    IsLeaf = false,
                    Items = item.Select(x => new MultiSelectViewModel
                    {
                        Id = x.Id,
                        Text = x.Name,
                        Checked = true,
                        IsLeaf = true
                    }).ToList()
                }).ToList(),
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
        public async Task<IActionResult> Edit([FromBody] EditChatGroupMapping model)
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
        public async Task<IActionResult> UpdateActive([FromBody] UpdateActiveConversationGroup model)
        {
            model.UserID = napSession.UserId;
            return Json(await chatGroupMappingService.UpdateActiveAsync(model));
        }
    }
}