using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Swagger.Attributes;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Http;

namespace Fanex.NCB.Swagger.Controllers
{
    //[CustomAuthorize]

    public class UserResourceController : ApiController
    {
        private readonly IUserService _userService;
        public UserResourceController()
        {
            _userService = new UserService();
        }
        // GET: api/ExcelHandler/5
        [HttpGet]
        public dynamic Query(int id, [FromUri]int siteId, [FromUri] string env)
        {
            var menuItems = _userService.GetMenuItem(id, siteId, env, true);
            if (menuItems.Any())
            {
                return new
                {
                    Menu = menuItems.Select(x => CreateMenu(x)).ToList()
                };
            }

            return null;
        }

        private dynamic CreateMenu(MenuItem x)
        {
            if (x != null)
            {
                return new
                {
                    x.Url,
                    x.DisplayOrder,
                    x.Policy,
                    x.Name,
                    Parent = CreateMenu(x.Parent)
                };
            }
            return null;
        }
    }
}
