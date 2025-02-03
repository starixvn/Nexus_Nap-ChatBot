namespace Fanex.NCB.NAPResolver.LocalServices;

using System;
using System.Globalization;
using System.Threading.Tasks;
using Fanex.NAP.Client.Models.ParameterObject;
using Fanex.NAP.Client.Models.ValueObject;
using NAP.Client.Services;
using Core.Enum;
using Criteria.LocalNap.Security;
using Criteria.LocalNap.Site;
using Shared.Services.Interfaces;

public class LocalSecurityService : ISecurityService
{
    private readonly IBaseService baseService;

    public LocalSecurityService(IBaseService baseService)
    {
        this.baseService = baseService;
    }

    public string GetTime()
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetTimeAsync()
    {
        var criteria = new UserSsoGetTimeUserCriteria
        {
            Time = DateTimeOffset.Now.DateTime
        };

        await baseService.GetDataAsync<int>(criteria);
        return criteria.Time.ToString(CultureInfo.InvariantCulture);
    }

    public int Login(LoginParameter parameter)
    {
        throw new NotImplementedException();
    }

    public async Task<int> LoginAsync(LoginParameter parameter)
    {
        var onUserId = await UserCheckIn(parameter);
        if (onUserId > 0) _ = LogUserLoginAsync(parameter);

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
            CountryCode = parameter.CountryCode
        };
        await baseService.GetDataAsync<int>(logCriteria);
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

        await baseService.GetDataAsync<int>(criteria);

        return criteria.OnUserId;
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

        await baseService.GetDataAsync<int>(criteria);
    }

    public int ReCheckIn(int onlineUserId, int siteId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ReCheckInAsync(int onlineUserId, int siteId)
    {
        var isActivate = await IsActivatedAsync(siteId);

        if (!isActivate) return (int)RecheckInStatus.KickOut;

        var criteria = new UserReCheckInUserCriteria
        {
            OnUserId = onlineUserId,
            IsKickedOut = false
        };

        await baseService.GetDataAsync<int>(criteria);

        if (criteria.IsPermissionChanged) return (int)RecheckInStatus.KickOutPermission;

        if (criteria.IsKickedOut) return (int)RecheckInStatus.KickOut;

        return (int)RecheckInStatus.Success;
    }

    public int SSOLogin(SSOLoginParameter parameter)
    {
        throw new NotImplementedException();
    }

    public async Task<int> SSOLoginAsync(SSOLoginParameter parameter)
    {
        var onUserId = await SSOCheckIn(parameter);
        if (onUserId > 0) await LogSSOLoginAsync(parameter);

        return onUserId;
    }

    private async Task<int> SSOCheckIn(SSOLoginParameter parameter)
    {
        var criteria = new UserSsoCheckInUserCriteria
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
        await baseService.GetDataAsync<int>(criteria);
        return criteria.OnUserId;
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
            CountryCode = parameter.CountryCode
        };

        await baseService.GetDataAsync<int>(logCriteria);
    }

    public bool UpdatePassword(int siteId, int userId, string password)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdatePasswordAsync(int siteId, int userId, string password)
    {
        throw new NotImplementedException();
    }

    private async Task<bool> IsActivatedAsync(int siteId)
    {
        var criteria = new GetByIdSiteCriteria
        {
            SiteId = siteId
        };

        var isActive = false;
        var site = await baseService.GetDataEntityAsync<SiteUrl>(criteria);

        if (site != null) isActive = site.IsActive;

        return isActive;
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