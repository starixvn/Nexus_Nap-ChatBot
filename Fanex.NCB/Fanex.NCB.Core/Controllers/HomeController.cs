using System.Diagnostics;
using System.Threading.Tasks;
using Fanex.NCB.Core.Models;
using Fanex.NCB.Core.Models.DataDomain;
using Fanex.NCB.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using Fanex.NCB.Main.Models.DTO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Fanex.NCB.Core.Models.DTO;
using System;
using System.Net.Http;

namespace Fanex.NCB.Main.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IBridgeClientService _bridgeClient;
        private readonly AppSettings _options;
        public HomeController(ISessionService sessionService, IBridgeClientService bridgeClient, IOptions<AppSettings> options)
        {
            _sessionService = sessionService;
            _bridgeClient = bridgeClient;
            _options = options.Value;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var userData = JsonConvert.DeserializeObject<ApplicationUser>(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value);
            //fetch menu & permission
            //var apiUrl = $"{_options.UserResourceApi}/Query/{userData.UserId}?siteid={_options.SiteId}&env={_options.Env}";
            //var response = await _bridgeClient.GetJsonAsync(apiUrl);

            //response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            //var item = await response.Content.ReadAsStringAsync();

            //var jsonObj = JObject.Parse(item);
            var menuList = new List<Menu>();
            //foreach (var m in jsonObj["Menu"])
            //{
                //menuList.Add(ParserMenu(m));
            //}
            ViewBag.Menu = menuList;
            return View();
        }

        private Menu ParserMenu(JToken m)
        {
            return new Menu
            {
                Url = m["Url"].Value<string>(),
                DisplayOrder = m["DisplayOrder"].Value<short>(),
                Policy = m["Policy"].Value<int>(),
                Name = m["Name"].Value<string>(),
                Parent = m["Parent"].HasValues ? ParserMenu(m["Parent"]) : null
            };
        }    
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
