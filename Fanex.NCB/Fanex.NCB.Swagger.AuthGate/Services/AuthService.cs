using System;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Common.MetalModels;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;


        public AuthService(IUserService userService)
        {
            _userService = userService;
        }
        public ApplicationUser Authorize(AuthenticationUser authenticationUser)
        {
            var userInfo = _userService.Get(authenticationUser.UserName);
            
            if (userInfo != null && authenticationUser.Password.Equals(userInfo.Password))
            {
                var result = new ApplicationUser
                {
                    FullName = string.Join(" ", userInfo.FirstName, userInfo.LastName),
                    UserId=userInfo.Id,
                    UserName=userInfo.Username                   
                };
               // var permission = _userService.
            }

            return null;
        }
    }
}