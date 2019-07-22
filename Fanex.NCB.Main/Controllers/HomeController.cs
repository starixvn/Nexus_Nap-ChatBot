using Fanex.NCB.Common.DTO;
using Fanex.NCB.Core.Models;
using Fanex.NCB.Core.Models.DataDomain;
using Fanex.NCB.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fanex.NCB.Main.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IBridgeClientService _bridgeClient;
        private readonly AppSettings _options;

        public HomeController(IBridgeClientService bridgeClient, IOptions<AppSettings> options)
        {
            _bridgeClient = bridgeClient;
            _options = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userData = JsonConvert.DeserializeObject<ApplicationUser>(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value);
            var apiUrl = $"{_options.UserResourceApi}/Query/{userData.UserId}?siteid={_options.SiteId}&env={_options.Env}";
            var response = await _bridgeClient.GetJsonAsync(apiUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            ViewBag.Menu = await response.Content.ReadAsAsync<List<Menu>>().ConfigureAwait(false);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}