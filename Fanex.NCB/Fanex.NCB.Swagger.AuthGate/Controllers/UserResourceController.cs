using System.Web.Http;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Swagger.AuthGate.Attributes;
using Fanex.NCB.Swagger.AuthGate.Services;

namespace Fanex.NCB.Swagger.AuthGate.Controllers
{
    [CustomAuthorize]
    public class UserResourceController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;

        public UserResourceController(IUserService userService, IMenuService menuService)
        {
            _userService = userService;
            _menuService = menuService;
        }

        [HttpGet]
        public dynamic Query(int id, [FromUri]int siteId, [FromUri] string env)
        {
            var menuItems = _userService.GetMenuItem(id, siteId, env, true);
            return _menuService.GetMenus(menuItems);            
        }        
    }
}
