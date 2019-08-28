namespace Fanex.NCB.NAPResolver.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Core.Business;
    using Fanex.NCB.Criteria.LocalNap.User;

    public class LocalUserService : IUserService
    {
        public User Get(int userId)
        {
            throw new NotImplementedException();
        }

        public User Get(string username)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = objectDb.QueryEntity<User, User, User, User>(
                  (user, creator, modifier) =>
                  {
                      user.Creator = creator;
                      user.Modifier = modifier;

                      return user;
                  },
                  "CreatorId, ModifierId",
                  criteria);

                return result;
            }
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

        public async Task<User> GetAsync(string username)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = await objectDb.QueryEntityAsync<User, User, User, User>(
                  (user, creator, modifier) =>
                  {
                      user.Creator = creator;
                      user.Modifier = modifier;

                      return user;
                  },
                  "CreatorId, ModifierId",
                  criteria).ConfigureAwait(false);

                return result;
            }
        }

        public IEnumerable<MenuItem> GetMenuItem(int userId, int siteId, string environment = "UAT", bool? isActive = null)
        {
            var criteria = new GetMenuItemCriteria
            {
                UserId = userId,
                SiteId = siteId,
                Environment = environment
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = objectDb.Query<MenuItem, MenuItem, Function, Site, User, User, MenuItem>(
                  (menu, parent, function, site, creator, modififer) =>
                  {
                      menu.Parent = parent;
                      menu.Function = function;
                      menu.Site = site;
                      menu.Creator = creator;
                      menu.Modifier = modififer;

                      return menu;
                  },
                  "ParentId, FunctionId, SiteId, CreatorId, ModifierId",
                  criteria);

                return result;
            }
        }

        public async Task<IEnumerable<MenuItem>> GetMenuItemAsync(int userId, int siteId, string environment = "UAT", bool? isActive = null)
        {
            var criteria = new GetMenuItemCriteria
            {
                UserId = userId,
                SiteId = siteId,
                Environment = environment
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = await objectDb.QueryAsync<MenuItem, MenuItem, Function, Site, User, User, MenuItem>(
                  (menu, parent, function, site, creator, modififer) =>
                  {
                      menu.Parent = parent;
                      menu.Function = function;
                      menu.Site = site;
                      menu.Creator = creator;
                      menu.Modifier = modififer;

                      return menu;
                  },
                  "ParentId, FunctionId, SiteId, CreatorId, ModifierId",
                  criteria).ConfigureAwait(false);

                return result;
            }
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
            var criteria = new GetSiteLevelCriteria
            {
                UserId = userId,
                Environment = environment
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = objectDb.Query<SiteLevel, Site, User, User, SiteLevel>(
                  (siteLevel, site, creator, modifier) =>
                  {
                      siteLevel.Site = site;
                      siteLevel.Creator = creator;
                      siteLevel.Modifier = modifier;
                      return siteLevel;
                  },
                  "SiteId, CreatorId, ModifierId",
                  criteria);

                return result;
            }
        }

        public async Task<IEnumerable<SiteLevel>> GetSiteLevelAsync(int userId, string environment = "UAT")
        {
            var criteria = new GetSiteLevelCriteria
            {
                UserId = userId,
                Environment = environment
            };

            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                var result = await objectDb.QueryAsync<SiteLevel, Site, User, User, SiteLevel>(
                  (siteLevel, site, creator, modifier) =>
                  {
                      siteLevel.Site = site;
                      siteLevel.Creator = creator;
                      siteLevel.Modifier = modifier;
                      return siteLevel;
                  },
                  "SiteId, CreatorId, ModifierId",
                  criteria).ConfigureAwait(false);

                return result;
            }
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