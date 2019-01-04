using Fanex.NCB.Swagger.AuthGate.Models.DTO;
using Fanex.NCB.Swagger.AuthGate.Models.MetalModel;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public interface IAuthService
    {
        AuthUserInfo Authorize(AuthenticationUser authenticationUser);
    }
}
