using System.Web.Http;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Common.MetalModels;
using Fanex.NCB.Swagger.AuthGate.Attributes;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Fanex.NCB.Swagger.AuthGate.Controllers
{

    [CustomAuthorize]
    public class AuthController : ApiController
    {
        private readonly IUserService _userService;


        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public dynamic Verify(AuthenticationUser user)
        {

            var userInfo = _userService.Get(user.UserName);
            if (userInfo != null)
            {
                //verify user
                return new
                {
                    UserName = userInfo.Username,
                    FullName = string.Join(" ", userInfo.FirstName, userInfo.LastName),
                    UserId = userInfo.Id,
                    userInfo.Group,
                    userInfo.Password
                };
            }

            return null;
        }

    }
}