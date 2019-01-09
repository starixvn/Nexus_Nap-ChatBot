using System.Web.Http;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Common.MetalModels;
using Fanex.NCB.Swagger.AuthGate.Attributes;
using Fanex.NCB.Swagger.AuthGate.Services;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Fanex.NCB.Swagger.AuthGate.Controllers
{

    //[CustomAuthorize]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;


        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public dynamic Verify(AuthenticationUser user)
        {
            return _authService.Authorize(user);
        }

    }
}