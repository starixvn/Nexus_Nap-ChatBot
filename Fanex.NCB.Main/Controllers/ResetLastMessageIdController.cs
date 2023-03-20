namespace Fanex.NCB.Main.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Extensions;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ResetLastMessageIdController : Controller
    {
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly IUserService userService;
        private readonly IResetLastMessageIdService resetLastMessageIdService;
        private readonly ILogService logService;

        public ResetLastMessageIdController(
            INapSettings napSettings,
            INapSession napSession,
            IUserService userService,
            IResetLastMessageIdService resetLastMessageIdService,
            ILogService logService
        )
        {
            this.napSettings = napSettings;
            this.napSession = napSession;
            this.userService = userService;
            this.resetLastMessageIdService = resetLastMessageIdService;
            this.logService = logService;
        }

        [HttpGet]
        [Route("SettingConfiguration/ResetLastId")]
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
            await logService.WriteMessageSentryErrorAsync("go to action here!");
            try
            {
                var (currentMaxMessageInfos, messageTypeInfos) = await resetLastMessageIdService.GetCurrentPreviousMaxMessageAsync();
                await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(currentMaxMessageInfos));
                await logService.WriteMessageSentryErrorAsync(XmlHelper<CurrentMaxMessageInfo>.Serialize(currentMaxMessageInfos.ToList()));
                await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(messageTypeInfos));

                var abnormalBetsMissingMsg = await resetLastMessageIdService.GetAbnormalBetsMissingMessageAsync(currentMaxMessageInfos);
                await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(abnormalBetsMissingMsg));

                var missingMsgInfo = await resetLastMessageIdService.GetMissingMessageInfoAsync(currentMaxMessageInfos, messageTypeInfos, abnormalBetsMissingMsg);
                await logService.WriteMessageSentryErrorAsync(JsonConvert.SerializeObject(missingMsgInfo));

                return Json(new
                {
                    data = currentMaxMessageInfos,
                    internalMissingMsg = abnormalBetsMissingMsg,
                    missingMsgInfo
                });
            }
            catch (System.Exception ex)
            {
                await logService.WriteExceptionSentryErrorAsync(ex);
                throw;
            }
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Host)]
        public async Task<IActionResult> CreateMissingMessage([FromBody] List<MissingMesssageInfo> listMsg)
        {
            var commandResult = await resetLastMessageIdService.CreateMissingMessageAsync(listMsg);

            return Json(commandResult);
        }
    }
}