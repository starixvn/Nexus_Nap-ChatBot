namespace Fanex.NCB.NAPResolver.LocalServices
{
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Core.Business;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class LocalUserService : IUserService
    {
        public User Get(int userId)
        {
            throw new NotImplementedException();
        }

        public User Get(string username)
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

        public Task<User> GetAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetAsync(string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MenuItem> GetMenuItem(int userId, int siteId, string environment = "UAT", bool? isActive = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MenuItem>> GetMenuItemAsync(int userId, int siteId, string environment = "UAT", bool? isActive = null)
        {
            throw new NotImplementedException();
        }

        public int GetPermission(int userId, int siteId, string functionKey)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPermissionAsync(int userId, int siteId, string functionKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SiteLevel> GetSiteLevel(int userId, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SiteLevel>> GetSiteLevelAsync(int userId, string environment = "UAT")
        {
            throw new NotImplementedException();
        }

        public int GetUserLevel(int userId, int siteId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetUserLevelAsync(int userId, int siteId)
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

        public bool IsHavingPermission(int userId, int siteId, string functionKey, Constant.Policy policy)
        {
            throw new NotImplementedException();
        }
    }
}