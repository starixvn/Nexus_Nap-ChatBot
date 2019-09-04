namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Criteria.LocalNap.Security;
    using Fanex.NCB.Criteria.LocalNap.Site;

    public class LocalSecurityService : ISecurityService
    {
        public string GetTime()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTimeAsync()
        {
            var criteria = new UserSSOGetTimeUserCriteria
            {
                Time = DateTime.Now
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                await objectDb.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

                return criteria.Time.ToString(CultureInfo.InvariantCulture);
            }
        }

        public int Login(LoginParameter parameter)
        {
            throw new NotImplementedException();
        }

        public async Task<int> LoginAsync(LoginParameter parameter)
        {
            var onUserId = await UserCheckIn(parameter);
            if (onUserId > 0)
            {
                await LogUserLoginAsync(parameter);
            }

            return onUserId;
        }

        private async Task LogUserLoginAsync(LoginParameter parameter)
        {
            var logCriteria = new LogUserLoginCriteria
            {
                UserId = parameter.UserId,
                SiteId = parameter.SiteId,
                LogDate = parameter.LoginDate,
                DomainName = parameter.Domain,
                BrowserName = parameter.BrowserName,
                Platform = parameter.Platform,
                IP = parameter.Ip,
                CountryCode = parameter.CountryCode,
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(logCriteria.GetSettingKey()))
            {
                await objectDb.ExecuteNonQueryAsync(logCriteria).ConfigureAwait(false);
            }
        }

        private async Task<int> UserCheckIn(LoginParameter parameter)
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
                return criteria.OnUserId;
            }
        }

        public void Logout(int onlineUserId)
        {
            throw new NotImplementedException();
        }

        public async Task LogoutAsync(int onlineUserId)
        {
            var criteria = new UserLogOutCriteria
            {
                OnUserId = onlineUserId
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                await objectDb.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }
        }

        public int ReCheckIn(int onlineUserId, int siteId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> ReCheckInAsync(int onlineUserId, int siteId)
        {
            var isActivate = await IsActivatedAsync(siteId);

            if (!isActivate)
            {
                return (int)RecheckInStatus.KickOut;
            }

            var criteria = new UserReCheckInUserCriteria
            {
                OnUserId = onlineUserId,
                IsKickedOut = false
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                await objectDb.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }

            if (criteria.IsKickedOut)
            {
                return (int)RecheckInStatus.KickOut;
            }

            return (int)RecheckInStatus.Success;
        }

        public int SSOLogin(SSOLoginParameter parameter)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SSOLoginAsync(SSOLoginParameter parameter)
        {
            int onUserId = await SSOCheckIn(parameter);
            if (onUserId > 0)
            {
                await LogSSOLoginAsync(parameter);
            }

            return onUserId;
        }

        private async Task<int> SSOCheckIn(SSOLoginParameter parameter)
        {
            var criteria = new UserSSOCheckInUserCriteria
            {
                UserId = parameter.UserId,
                SiteId = parameter.SiteId,
                CheckOnUserId = parameter.CheckOnUserId,
                IP = parameter.IP,
                BrowserName = parameter.BrowserName,
                NetSessionId = parameter.NetSessionId,
                Platform = parameter.Platform,
                LoginDate = parameter.LoginDate
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                await objectDb.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
                return criteria.OnUserId;
            }
        }

        private async Task LogSSOLoginAsync(SSOLoginParameter parameter)
        {
            var logCriteria = new LogUserLoginCriteria
            {
                UserId = parameter.UserId,
                SiteId = parameter.SiteId,
                LogDate = parameter.LoginDate,
                DomainName = parameter.Domain,
                BrowserName = parameter.BrowserName,
                Platform = parameter.Platform,
                IP = parameter.IP,
                CountryCode = parameter.CountryCode,
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(logCriteria.GetSettingKey()))
            {
                await objectDb.ExecuteNonQueryAsync(logCriteria).ConfigureAwait(false);
            }
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

        private async Task<bool> IsActivatedAsync(int siteId)
        {
            var criteria = new GetByIdSiteCriteria
            {
                SiteId = siteId
            };

            bool isActive = false;
            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var site = await objectDb.QueryEntityAsync<Site>(criteria);

                if (site != null)
                {
                    isActive = site.IsActive;
                }

                return isActive;
            }
        }
    }
}