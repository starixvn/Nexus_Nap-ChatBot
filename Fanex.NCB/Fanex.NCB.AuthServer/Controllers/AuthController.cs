using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.AuthServer.Models;
using Fanex.NCB.AuthServer.Models.MetalModel;
using System.Collections.Generic;
using System.Web.Http;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Fanex.NCB.AuthServer.Controllers
{

    [CustomAuthorize]
    public class AuthController : ApiController
    {
        private readonly IUserService _userService;
        public AuthController()
        {
            _userService = new UserService();
        }

        // GET: Auth
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
                    Menu = new List<string>() { "menu1", "menu2" },
                    Permissions = new List<string>() { "key1", "key2" },
                    Group = "Administrator"
                });
            }

            return null;
        }

    }
}