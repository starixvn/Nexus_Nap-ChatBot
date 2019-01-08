using System;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Common.MetalModels;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public class AuthService : IAuthService
    {
        public ApplicationUser Authorize(AuthenticationUser authenticationUser)
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