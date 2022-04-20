namespace Fanex.NCB.Main.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ResetLastMessageIdController : Controller
    {
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly IUserService userService;
        private readonly IResetLastMessageIdService resetLastMessageIdService;

        public ResetLastMessageIdController(
            INapSettings napSettings,
            INapSession napSession,
            IUserService userService,
            IResetLastMessageIdService resetLastMessageIdService
        )
        {
            this.napSettings = napSettings;
            this.napSession = napSession;
            this.userService = userService;
            this.resetLastMessageIdService = resetLastMessageIdService;
        }

        [HttpGet]
        [Route("SettingConfiguration/ResetLastMessageId")]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Read)]
        public IActionResult Index()
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(napSession.UserId, napSettings.SiteId, FunctionKey.ResetLastMessageId, Policy.Host);
            
            return View("~/Views/SkypeBot/ResetLastMessageId/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Read)]
        public async Task<IActionResult> GetCurrentPreviousMaxMessage()
        {
            var (currentMaxMessageInfos, messageTypeInfos) = await resetLastMessageIdService.GetCurrentPreviousMaxMessageAsync();

            return Json(new { data = currentMaxMessageInfos, messageTypeInfos });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Read)]
        public async Task<IActionResult> GetNewPreviousMaxMessage([FromBody]GetNewPreviousMaxMessageCriteria criteria)
        {
            var (messagePostTimeInfos, newMaxMessageId) = await resetLastMessageIdService.GetNewPreviousMaxMessageAsync(criteria);

            return Json(new { messagePostTimeInfos, newMaxMessageId });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Read)]
        public async Task<IActionResult> GetMissingMessageCsd([FromBody]GetMissingMesssageCsdCriteria criteria)
        {
            var missingMessageCsd = await resetLastMessageIdService.GetMissingMessageCsdAsync(criteria);

            return Json(missingMessageCsd);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Read)]
        public async Task<IActionResult> GetMissingMessageInfo([FromBody]GetMissingMessageInfoCriteria criteria)
        {
            var missingMessageInfos = await resetLastMessageIdService.GetMissingMessageInfoAsync(criteria);

            return Json(missingMessageInfos);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Read)]
        public async Task<IActionResult> GetMissingProblemAccount([FromBody] GetMissingProblemAccountCriteria criteria)
        {
            var (logId, missingProblemAccount) = await resetLastMessageIdService.GetMissingProblemAccountAsync(criteria);

            return Json(new { logId, data = missingProblemAccount });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Host)]
        public async Task<IActionResult> CreateMissingMessage([FromBody]CreateMissingMessageCommand command)
        {
            var commandResult = await resetLastMessageIdService.CreateMissingMessageAsync(command);
            
            return Json(commandResult);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Host)]
        public async Task<IActionResult> EditMessageInfo([FromBody] EditMessageInfoCommand command)
        {
            var commandResult = await resetLastMessageIdService.EditMessageInfoAsync(command);

            return Json(commandResult);
        }
    }
}
