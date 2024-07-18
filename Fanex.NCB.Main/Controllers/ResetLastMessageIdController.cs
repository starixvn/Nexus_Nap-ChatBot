namespace Fanex.NCB.Main.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Enum;
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
            IResetLastMessageIdService resetLastMessageIdService)
        {
            this.napSettings = napSettings;
            this.napSession = napSession;
            this.userService = userService;
            this.resetLastMessageIdService = resetLastMessageIdService;
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
            var (currentMaxMessageInfos, messageTypeInfos) = await resetLastMessageIdService.GetCurrentPreviousMaxMessageAsync();
            var abnormalBetsMissingMsg = await resetLastMessageIdService.GetAbnormalBetsMissingMessageAsync(currentMaxMessageInfos);
            var missingMsgInfo = await resetLastMessageIdService.GetMissingMessageInfoAsync(currentMaxMessageInfos, messageTypeInfos, abnormalBetsMissingMsg);

            var messageInfosList = currentMaxMessageInfos.ToList();
            var index = messageInfosList.FindIndex(item => (JobIdentity)item.JobIdentify == JobIdentity.ResultAmendmentInternalCSD && item.SettingName == "LastModifiedTime");

            if (index >= 0)
            {
                var messageInfo = messageInfosList[index];
                messageInfo.ReferenceValue = ParseAndFormatDate(messageInfo.ReferenceValue);
                messageInfo.MaxReferenceId = ParseAndFormatDate(messageInfo.MaxReferenceId);
            }

            return Json(new
            {
                data = messageInfosList,
                internalMissingMsg = abnormalBetsMissingMsg,
                missingMsgInfo
            });
        }

        private static string ParseAndFormatDate(string dateString)
        {
            return DateTime.ParseExact(dateString, "MM/dd/yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)
                           .ToString("MM/dd/yyyy hh:mm:ss.fff tt", CultureInfo.InvariantCulture);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.ResetLastMessageId, Policy = Policy.Host)]
        public async Task<IActionResult> CreateMissingMessage([FromBody] List<MissingInfoIns> listMsg)
        {
            var commandResult = await resetLastMessageIdService.CreateMissingMessageAsync(listMsg);

            return Json(commandResult);
        }
    }
}