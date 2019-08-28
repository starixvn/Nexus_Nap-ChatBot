namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Criteria.LocalNap.Security;

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

        public async Task<int> LoginAsync(LoginParameter parameter)
        {
            var criteria = new UserCheckInCriteria
            {
                UserId = parameter.UserId,
                SiteId = parameter.SiteId,
                NetSessionId = parameter.NetSessionId,
                Ip = parameter.Ip,
                LoginDate = parameter.LoginDate
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                await objectDb.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

                LogUser(parameter);

                return criteria.OnUserId;
            }
        }

        private void LogUser(LoginParameter loginParameter)
        {
            var logCriteria = new LogUserLoginCriteria
            {
                UserId = loginParameter.UserId,
                SiteId = loginParameter.SiteId,
                LogDate = loginParameter.LoginDate,
                DomainName = loginParameter.Domain,
                BrowserName = loginParameter.BrowserName,
                Platform = loginParameter.Platform,
                IP = loginParameter.Ip,
                CountryCode = loginParameter.CountryCode,
            };
            using (var objectDb = ObjectDbFactory.CreateInstance(logCriteria.GetSettingKey()))
            {
                objectDb.ExecuteNonQuery(logCriteria);
            }
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