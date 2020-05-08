namespace Fanex.NCB.Main.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.SettingConfiguration;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;

    [NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SettingConfigurationController : Controller
    {
        private readonly INapSession napSession;
        private readonly ISettingConfigurationService settingConfigurationService;

        public SettingConfigurationController(INapSession napSession, ISettingConfigurationService settingConfigurationService)
        {
            this.napSession = napSession;
            this.settingConfigurationService = settingConfigurationService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingManagement, Policy = Policy.Read)]
        public IActionResult Index([FromServices]IUserService userService)
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.SettingManagement, Policy.Host);

            return View("~/Views/SkypeBot/SettingConfiguration/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetSettings(int pageIndex, int pageSize, int? settingType)
        {
            var result = await settingConfigurationService.GetSettingsAsync(pageIndex, pageSize, settingType).ConfigureAwait(false);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody]CreateSettingTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await settingConfigurationService.CreateAsync(new CreateSettingType
                {
                    Type = viewModel.Type,
                    Name = viewModel.Name,
                    CreatorId = napSession.UserId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var message = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
            return Json(new { isSuccessful = false, message });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetByType(int type)
        {
            var result = await settingConfigurationService.GetByTypeAsync(type).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody]CreateSettingTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await settingConfigurationService.UpdateAsync(new UpdateSettingType
                {
                    Type = viewModel.Type,
                    Name = viewModel.Name,
                    ModifiedBy = napSession.UserId,
                }).ConfigureAwait(false);

                return Json(result);
            }

            var message = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
            return Json(new { isSuccessful = false, message });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int type)
        {
            if (ModelState.IsValid)
            {
                var result = await settingConfigurationService.DeleteAsync(type, napSession.UserId).ConfigureAwait(false);

                return Json(result);
            }

            var message = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
            return Json(new { isSuccessful = false, message });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Read)]
        public IActionResult Mapping([FromServices]IUserService userService)
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.MappingSettingManagement, Policy.Host);

            return View("~/Views/SkypeBot/SettingConfiguration/Mapping.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetSettingTypes()
        {
            var types = await settingConfigurationService.GetAllSettingType().ConfigureAwait(false);

            return Json(types);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetJobInfo(int? jobId)
        {
            var value = jobId ?? 1;
            var jobInfo = await settingConfigurationService.GetJobInfoAsync(value).ConfigureAwait(false);

            return Json(jobInfo);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetMappings(int jobId, int pageIndex, int pageSize)
        {
            var result = await settingConfigurationService.GetMappingsAsync(jobId, pageIndex, pageSize);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateJobInfo([FromBody] UpdateJobInfoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await settingConfigurationService.UpdateJobInfoAsync(new UpdateJobInfo
                {
                    Id = viewModel.Id,
                    BatchSize = viewModel.BatchSize,
                    TestingGroup = viewModel.TestingGroup,
                    ModifiedBy = napSession.UserId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var message = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
            return Json(new { isSuccessful = false, message });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetMsgContentsTypes()
        {
            var result = await settingConfigurationService.GetMsgContentsTypesAsync();

            return Json(result);
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Read)]
        public async Task<IActionResult> GetNotificationTypeData(int notificationTypeId)
        {
            var result = await settingConfigurationService.GetNotificationTypeDataAsync(notificationTypeId);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> CreateMapping([FromBody] CreateMappingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await settingConfigurationService.CreateMappingAsync(new CreateMapping
                {
                    JobId = viewModel.JobId,
                    NotificationTypeID = viewModel.NotificationTypeId,
                    NotificationTemplateID = viewModel.NotificationTemplateId,
                    MsgContentsType = viewModel.MsgContentsType,
                    RequiredParams = viewModel.RequiredParam,
                    IsTest = viewModel.IsTest,
                    CreatorId = napSession.UserId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var message = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
            return Json(new { isSuccessful = false, message });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> GetMapping(int msgContentsType, int jobId)
        {
            var model = new GetMapping
            {
                MsgContentsType = msgContentsType,
                JobId = jobId
            };

            var result = await settingConfigurationService.GetMappingAsync(model);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateMapping([FromBody] UpdateMappingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await settingConfigurationService.UpdateMappingAsync(new UpdateMapping
                {
                    JobId = viewModel.JobId,
                    NotificationTypeID = viewModel.NotificationTypeId,
                    NotificationTemplateID = viewModel.NotificationTemplateId,
                    MsgContentsType = viewModel.MsgContentsType,
                    RequiredParams = viewModel.RequiredParam,
                    IsTest = viewModel.IsTest,
                    OldMsgContentsType = viewModel.OldMsgContentsType,
                    ModifiedBy = napSession.UserId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var message = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
            return Json(new { isSuccessful = false, message });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.MappingSettingManagement, Policy = Policy.Host)]
        public async Task<IActionResult> DeleteMapping(int templateId, int msgContentsType, int jobId)
        {
            if (ModelState.IsValid)
            {
                var model = new DeleteMapping
                {
                    TemlateId = templateId,
                    MsgContentsType = msgContentsType,
                    JobId = jobId,
                    ModifiedBy = napSession.UserId
                };

                var result = await settingConfigurationService.DeleteMappingAsync(model);

                return Json(result);
            }

            var message = ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
            return Json(new { isSuccessful = false, message });
        }
    }
}