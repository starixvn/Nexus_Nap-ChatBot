namespace Fanex.NCB.Main.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.SettingConfiguration;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.NAPResolver.Attributes;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using static Fanex.NAP.Core.Business.Constant;
    using System.Linq;

    //[NapChatBotSuperAuth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SettingConfigurationController : Controller
    {
        private readonly INapSession _napSession;
        private readonly ISettingConfigurationService _settingConfigurationService;

        public SettingConfigurationController(INapSession napSession, ISettingConfigurationService settingConfigurationService)
        {
            _napSession = napSession;
            _settingConfigurationService = settingConfigurationService;
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Read)]
        public IActionResult Index([FromServices]IUserService userService)
        {
            ViewBag.HaveHostPermission = userService.IsHavingPermission(_napSession.UserId, NapClientManager.NapSettings.SiteId, FunctionKey.SettingConfiguration, Policy.Host);

            return View("~/Views/SkypeBot/SettingConfiguration/Index.cshtml");
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetSettingTypes(int pageIndex, int pageSize, int? settingType)
        {
            var result = await _settingConfigurationService.GetSettingTypesAsync(pageIndex, pageSize).ConfigureAwait(false);

            return Json(new { total = result.Item1, data = result.Item2 });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody]CreateSettingTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _settingConfigurationService.CreateAsync(new CreateSettingType
                {
                    Type = viewModel.Type,
                    Name = viewModel.Name,
                    CreatorId = _napSession.UserId
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Read)]
        public async Task<IActionResult> GetByType(int type)
        {
            var result = await _settingConfigurationService.GetByTypeAsync(type).ConfigureAwait(false);

            return Json(result);
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody]CreateSettingTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _settingConfigurationService.UpdateAsync(new UpdateSettingType
                {
                    Type = viewModel.Type,
                    Name = viewModel.Name,
                    ModifiedBy = _napSession.UserId,
                }).ConfigureAwait(false);

                return Json(result);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }

        [HttpPost]
        [NapChatBotAuth(FunctionKey = FunctionKey.SettingConfiguration, Policy = Policy.Host)]
        public async Task<IActionResult> Delete(int type)
        {
            if (ModelState.IsValid)
            {
                var result = await _settingConfigurationService.DeleteAsync(type, _napSession.UserId).ConfigureAwait(false);

                return Json(result);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            return Json(new { isSuccessful = false, errors });
        }
    }
}