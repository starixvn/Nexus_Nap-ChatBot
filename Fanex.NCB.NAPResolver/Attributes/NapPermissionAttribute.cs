namespace Fanex.NCB.NAPResolver.Attributes
{
    using System;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using static Fanex.NAP.Core.Business.Constant;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NapPermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IMemoryCache cache;
        private readonly IUserService userService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;

        private readonly int cacheDuration;
        private readonly string functionKey;
        private readonly Policy policy;
        private readonly bool permissionCache;

        public NapPermissionAttribute(IUserService userService, INapSettings napSettings, INapSession napSession, string functionKey, Policy policy, bool permissionCache = false, int cacheDuration = 0)
        {
            this.userService = userService;
            this.napSettings = napSettings;
            this.napSession = napSession;
            this.cache = new MemoryCache(new MemoryCacheOptions());

            this.functionKey = functionKey;
            this.policy = policy;
            this.permissionCache = permissionCache;
            this.cacheDuration = cacheDuration;
        }

        public int CacheDuration
        {
            get => cacheDuration == 0 ? napSettings.CacheDuration : cacheDuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (UserHasInValidFunctionPolicy(napSession))
            {
                var unauthorizeUrl = napSettings.UnauthorizeUrl;
                context.Result = new RedirectResult(unauthorizeUrl);
            }
        }

        private bool UserHasInValidFunctionPolicy(INapSession currentSession)
        {
            int userPolicy = 0;

            if (permissionCache)
            {
                var cacheKey = string.Format("{0}_{1}_{2}_{3}", napSettings.AccessKey, currentSession.UserId,
                    napSettings.SiteId, functionKey);
                var cachedPolicy = cache.Get(cacheKey);

                if (cachedPolicy == null)
                {
                    userPolicy = userService.GetPermission(currentSession.UserId, napSettings.SiteId, functionKey);
                    cache.Set(cacheKey, userPolicy, DateTime.Now.AddSeconds(CacheDuration));
                }
                else
                {
                    userPolicy = (int)cachedPolicy;
                }
            }
            else
            {
                userPolicy = userService.GetPermission(currentSession.UserId, napSettings.SiteId, functionKey);
            }

            return userPolicy < (int)policy;
        }
    }
}