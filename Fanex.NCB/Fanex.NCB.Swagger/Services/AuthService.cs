using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fanex.NCB.AuthServer.Models.MetalModel;
using Fanex.NCB.Swagger.Models.DTO;

namespace Fanex.NCB.Swagger.Services
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