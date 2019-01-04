using System;
using Fanex.NCB.Swagger.AuthGate.Models.DTO;
using Fanex.NCB.Swagger.AuthGate.Models.MetalModel;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public class AuthService : IAuthService
    {
        public AuthUserInfo Authorize(AuthenticationUser authenticationUser)
        {
            //if(UM)
            //{
            //    //connect to LOCALDB
            //}else
            //{
            //    //connect to NAP
            //}
            throw new NotImplementedException();
        }
    }
}