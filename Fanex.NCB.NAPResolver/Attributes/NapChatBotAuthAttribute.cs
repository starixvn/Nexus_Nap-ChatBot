namespace Fanex.NCB.NAPResolver.Attributes;

using System;
using System.Threading.Tasks;
using NAP.Client;
using NAP.Client.Configuration;
using NAP.Client.Helpers.Web;
using NAP.Client.Services;
using LocalServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using static Fanex.NAP.Core.Business.Constant;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NapChatBotAuthAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly IMemoryCache cache;
    private int cacheDuration;
    private readonly IUserService userService;
    private readonly INapSettings napSettings;
    private readonly INapSession napSession;

    public NapChatBotAuthAttribute() : this(null, null, null, null)
    {
    }

    public NapChatBotAuthAttribute(IUserService userService, INapSettings napSettings, INapSession napSession,
        IMemoryCache cache)
    {
        this.userService = userService ?? GetUserService();
        this.napSettings = napSettings ?? NapClientManager.NapSettings;
        this.napSession = napSession ?? NapClientManager.NapSession;
        this.cache = cache ?? new MemoryCache(new MemoryCacheOptions());
    }

    public string FunctionKey { get; set; }

    public Policy Policy { get; set; }

    public bool PermissionCache { get; set; }

    public int CacheDuration
    {
        get => cacheDuration == 0 ? napSettings.CacheDuration : cacheDuration;
        set => cacheDuration = value;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context?.HttpContext == null)
        {
            RequestHelper.CloseConnection(context);
            return;
        }

        var hasValidFunctionPolicy = await UserHasValidFunctionPolicy(napSession);
        if (hasValidFunctionPolicy)
        {
            var unauthorizeUrl = napSettings.UnauthorizeUrl;
            context.Result = new RedirectResult(string.IsNullOrEmpty(unauthorizeUrl)
                ? "~/Error/UnauthorizedUser"
                : unauthorizeUrl);
        }
    }

    private async Task<bool> UserHasValidFunctionPolicy(INapSession currentSession)
    {
        int policy;
        if (PermissionCache)
        {
            var cacheKey = string.Format("{0}_{1}_{2}", currentSession.UserId,
                napSettings.SiteId, FunctionKey);
            var cachedPolicy = cache.Get(cacheKey);

            if (cachedPolicy == null)
            {
                policy = await userService.GetPermissionAsync(currentSession.UserId, napSettings.SiteId, FunctionKey);
                cache.Set(cacheKey, policy, DateTimeOffset.Now.DateTime.AddSeconds(CacheDuration));
            }
            else
            {
                policy = (int)cachedPolicy;
            }
        }
        else
        {
            policy = await userService.GetPermissionAsync(currentSession.UserId, napSettings.SiteId, FunctionKey);
        }

        return policy < (int)Policy;
    }

    private static IUserService GetUserService()
    {
        if (NapServicesRegistrationExtension.NAPResolverType == NapResolverType.Remote)
        {
            return new UserService();
        }
        else
        {
            return new LocalUserService();
        }
    }
}