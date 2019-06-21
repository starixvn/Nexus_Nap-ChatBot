using Fanex.NAP.Core.Models;
using Fanex.NCB.Swagger.AuthGate.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fanex.NCB.Swagger.AuthGate.UnitTests.Services
{
    public class MenuServiceTest
    {
        private readonly MenuService _menuService = null;

        public MenuServiceTest()
        {
            _menuService = new MenuService();
        }

        [Fact]
        public void GetMenu_WhenCalled_CompareNumberOfRootMenu_Correctly()
        {
            //Arrange
            var menuItemsFake = FakeMenus()?.ElementAtOrDefault(0);

            //Act
            var menu = _menuService.GetMenus(menuItemsFake);

            //Assert
            Assert.Equal(5, menu.Count);
        }

        [Fact]
        public void GetMenu_WhenCalled_CompareNumberOfChild_Correctly()
        {
            //Arrange
            var menuItemsFake = FakeMenus()?.ElementAtOrDefault(1);

            //Act
            var menu = _menuService.GetMenus(menuItemsFake);

            //Assert
            var numberOfHaveSubmenu = menu.Count(_ => _.SubMenus != null && _.SubMenus.Count > 0);
            Assert.Equal(2, numberOfHaveSubmenu);
        }

        private List<List<MenuItem>> FakeMenus()
        {
            var item1 = new MenuItem() { Id = 1 };
            var item2 = new MenuItem() { Id = 2 };
            var item3 = new MenuItem() { Id = 3 };
            var item4 = new MenuItem() { Id = 4 };
            var item5 = new MenuItem() { Id = 5 };

            var result = new List<List<MenuItem>>();
            result.Add(new List<MenuItem>()
            {
                item1,
                item2,
                item3,
                item4,
                item5
            });

            var item6 = new MenuItem() { Id = 6, Parent = item1 };
            var item7 = new MenuItem() { Id = 7, Parent = item2 };
            var item8 = new MenuItem() { Id = 8 };

            result.Add(new List<MenuItem>()
            {
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                item8
            });

            return result;
        }
    }
}