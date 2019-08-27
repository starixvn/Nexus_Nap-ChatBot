namespace Fanex.NCB.NAPResolver.LocalServices
{
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using System;
    using System.Threading.Tasks;

    public class LocalSecurityService : ISecurityService
    {
        public string GetTime()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTimeAsync()
        {
            throw new NotImplementedException();
        }

        public int Login(LoginParameter parameter)
        {
            throw new NotImplementedException();
        }

        public Task<int> LoginAsync(LoginParameter parameter)
        {
            throw new NotImplementedException();
        }

        public void Logout(int onlineUserId)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync(int onlineUserId)
        {
            throw new NotImplementedException();
        }

        public int ReCheckIn(int onlineUserId, int siteId)
        {
            throw new NotImplementedException();
        }

        public Task<int> ReCheckInAsync(int onlineUserId, int siteId)
        {
            throw new NotImplementedException();
        }

        public int SSOLogin(SSOLoginParameter parameter)
        {
            throw new NotImplementedException();
        }

        public Task<int> SSOLoginAsync(SSOLoginParameter parameter)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePassword(int siteId, int userId, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePasswordAsync(int siteId, int userId, string password)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProfile(int siteId, UpdateUserInfoParameter parameter)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProfileAsync(int siteId, UpdateUserInfoParameter parameter)
        {
            throw new NotImplementedException();
        }
    }
}