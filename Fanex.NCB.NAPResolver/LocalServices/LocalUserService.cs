namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data.Repository;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using NAP.Client.Services;
    using Fanex.NAP.Core.Business;
    using Criteria.LocalNap.User;
    using Model;
    using Shared.Services;
    using Shared.Services.Interfaces;

    public class LocalUserService : IUserService
    {
        private readonly IBaseService baseService;

        public LocalUserService(IBaseService baseService = null)
        {
            this.baseService = baseService ?? new BaseService(new DynamicRepository());
        }

        public User Get(int userId)
        {
            throw new NotImplementedException();
        }

        public User Get(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetAsync(int userId)
        {
            var criteria = new GetByUserIdCriteria
            {
                UserId = userId
            };

            var flatUser = await baseService.GetDataEntityAsync<FlatUser>(criteria);
            return flatUser.ConvertFlatUserToNestedModel();
        }

        public async Task<User> GetAsync(string username)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            var flatUser = await baseService.GetDataEntityAsync<FlatUser>(criteria);
            var user = flatUser.ConvertFlatUserToNestedModel();
            return user;
        }

        public IEnumerable<MenuItem> GetMenuItem(int userId, int siteId, string environment = "UAT", bool? isActive = null)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemAsync(int userId, int siteId, string environment = "UAT", bool? isActive = null)
        {
            var criteria = new GetMenuItemCriteria
            {
                UserId = userId,
                SiteId = siteId,
                Environment = environment
            };
            var flatMenus = await baseService.GetDataAsync<FlatMenuItem>(criteria);
            var menus = flatMenus.Select(i => i.ConvertToNestedModel());
            return menus;
        }

        public int GetPermission(int userId, int siteId, string functionKey)
        {
            var criteria = new GetUserFunctionPolicyCriteria
            {
                UserId = userId,
                SiteId = siteId,
                FunctionKey = functionKey
            };

            return baseService.GetDataEntity<int>(criteria);
        }

        public async Task<int> GetPermissionAsync(int userId, int siteId, string functionKey)
        {
            var criteria = new GetUserFunctionPolicyCriteria
            {
                UserId = userId,
                SiteId = siteId,
                FunctionKey = functionKey
            };

            return await baseService.GetDataEntityAsync<int>(criteria);
        }

        public IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public static Task<IEnumerable<FunctionPolicy>> GetPermissionsAsync(int userId, int siteId, string parentFunctionKey, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SiteLevel> GetSiteLevel(int userId, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SiteLevel>> GetSiteLevelAsync(int userId, string environment = "UAT")
        {
            var criteria = new GetSiteLevelCriteria
            {
                UserId = userId,
                IsActive = true,
                Environment = environment
            };

            var flatSiteLevels = await baseService.GetDataAsync<FlatSiteLevel>(criteria);

            return flatSiteLevels.Select(i => i.ConvertToNestedModel());
        }

        public int GetUserLevel(int userId, int siteId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUserLevelAsync(int userId, int siteId)
        {
            throw new NotImplementedException();
        }

        public static bool IgnoredOtp(int userId, string system)
        {
            throw new NotImplementedException();
        }

        public static Task<bool> IgnoredOtpAsync(int userId, string system)
        {
            throw new NotImplementedException();
        }

        public bool IsHavingPermission(int userId, int siteId, string functionKey, Constant.Policy policy)
        {
            var currentPolicy = GetPermission(userId, siteId, functionKey);

            return currentPolicy >= (int)policy;
        }

        public static Task<bool> IsHavingPermissionAsync(int userId, int siteId, string functionKey, Constant.Policy policy)
        {
            throw new NotImplementedException();
        }

        public static bool VerifyOtp(int userId, string system, string otp)
        {
            throw new NotImplementedException();
        }

        public static Task<bool> VerifyOtpAsync(int userId, string system, string otp)
        {
            throw new NotImplementedException();
        }

        public bool HasAnyPermission(int userId, int siteId, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasAnyPermissionAsync(int userId, int siteId, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}