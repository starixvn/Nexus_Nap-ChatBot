using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.AuthServer.Models;
using Fanex.NCB.AuthServer.Models.MetalModel;
using Fanex.NCB.AuthServer.Services;
using System.Collections.Generic;
using System.Web.Http;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Fanex.NCB.AuthServer.Controllers
{

    [CustomAuthorize]
    public class AuthController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IMenuService _menuService;


        public AuthController()
        {
            _userService = new UserService();
            _menuService = new MenuService();
        }

        [HttpPost]
        public dynamic Verify(AuthenticationUser user)
        {

            var userInfo = _userService.Get(user.UserName);
            if (userInfo != null)
            {
                return Json(new
                {
                    UserName = userInfo.Username,
                    FullName = string.Join(" ", userInfo.FirstName, userInfo.LastName),
                    Menu = _menuService.GetMenuMetaModels(userInfo.Id),
                    Permissions = new List<string>() { "key1", "key2" }
                });
            }

            return null;
        }

    }
}