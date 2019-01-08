using Fanex.NCB.Common.DTO;
using Fanex.NCB.Common.MetalModels;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public interface IAuthService
    {
        ApplicationUser Authorize(AuthenticationUser authenticationUser);
    }
}
