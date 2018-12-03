using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.AuthServer.Configurations;
using Fanex.NCB.AuthServer.Models.MetalModel;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NCB.AuthServer.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUserService _userService;
        private readonly IAppSettings _appSettings;

        public MenuService()
        {
            _userService = new UserService();
            _appSettings = new AppSettings();
        }

        public MenuService(
            IUserService userService,
            IAppSettings appSettings)
        {
            _userService = userService;
            _appSettings = appSettings;
        }

        public IEnumerable<MenuItem> GetMenuItems(int userId)
        {
            return _userService.GetMenuItem(userId, _appSettings.SiteId, _appSettings.Environment, true);
        }

        public IEnumerable<MenuMetaModel> GetMenuMetaModels(int userId)
        {
            var menuModels = new List<MenuMetaModel>();
            var menuItems = GetMenuItems(userId);
            var roots = menuItems.Where(m => m.Parent == null).OrderBy(m => m.DisplayOrder);

            foreach (MenuItem menu in roots)
            {
                var menuModel = new MenuMetaModel(menu);

                menuModels.Add(menuModel);
                AddChildMenuModel(menuItems, menuModel);
            }

            return menuModels;
        }

        private void AddChildMenuModel(IEnumerable<MenuItem> menus, MenuMetaModel parent)
        {
            var childMenus = menus.Where(m => m.Parent != null && m.Parent.Id == parent.Id).OrderBy(m => m.DisplayOrder);
            foreach (MenuItem menu in childMenus)
            {
                var menuModel = new MenuMetaModel(menu);

                parent.SubMenus.Add(menuModel);
                AddChildMenuModel(menus, menuModel);
            }
        }
    }
}