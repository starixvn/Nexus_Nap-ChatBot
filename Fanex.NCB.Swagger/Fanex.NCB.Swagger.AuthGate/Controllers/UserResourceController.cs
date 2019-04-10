using System.Web.Http;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Swagger.AuthGate.Attributes;
using Fanex.NCB.Swagger.AuthGate.Services;

namespace Fanex.NCB.Swagger.AuthGate.Controllers
{
    /// <summary>
    /// Resource API.
    /// </summary>
    [CustomAuthorize]
    public class UserResourceController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;

        /// <summary>
        ///  constructor
        /// </summary>
        /// <param name="userService">User Service</param>
        /// <param name="menuService">Menu Service</param>
        public UserResourceController(IUserService userService, IMenuService menuService)
        {
            _userService = userService;
            _menuService = menuService;
        }

        /// <summary>
        /// Fetch the menu of nap site.
        /// </summary>
        /// <param name="id">UserID</param>
        /// <param name="siteId">Site ID</param>
        /// <param name="env">Environment</param>
        /// <returns></returns>
        [HttpGet]
        public dynamic Query(int id, [FromUri]int siteId, [FromUri] string env)
        {
            var menuItems = _userService.GetMenuItem(id, siteId, env, true);
            return _menuService.GetMenus(menuItems);            
        }        
    }
}
