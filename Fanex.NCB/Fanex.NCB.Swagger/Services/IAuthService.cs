using Fanex.NCB.AuthServer.Models.MetalModel;
using Fanex.NCB.Swagger.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NCB.Swagger.Services
{
   public interface IAuthService
    {
        AuthUserInfo Authorize(AuthenticationUser authenticationUser);
    }
}
