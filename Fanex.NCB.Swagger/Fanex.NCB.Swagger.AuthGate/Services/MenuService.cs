using Fanex.NAP.Core.Models;
using Fanex.NCB.Common.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.NCB.Swagger.AuthGate.Services
{
    public class MenuService : IMenuService
    {
        public List<Menu> GetMenus(IEnumerable<MenuItem> menuItems)
        {
            var rootMenus = menuItems.Where(m => m.Parent == null).OrderBy(m => m.DisplayOrder);

            return BuildMenus(rootMenus, menuItems);
        }

        private List<Menu> GetSubMenus(IEnumerable<MenuItem> menuItems, int parentId)
        {
            var childMenus = menuItems.Where(m => m.Parent != null && m.Parent.Id == parentId).OrderBy(m => m.DisplayOrder);

            return BuildMenus(childMenus, menuItems);
        }

        private List<Menu> BuildMenus(IEnumerable<MenuItem> selectedMenuItems, IEnumerable<MenuItem> menuItems)
        {
            return selectedMenuItems
                .Select(m => new Menu
                {
                    Id = m.Id,
                    Name = m.Name,
                    Url = m.Url,
                    Policy = m.Policy,
                    SubMenus = GetSubMenus(menuItems, m.Id)
                })
                .ToList();
        }
    }
}