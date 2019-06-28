using AutoFixture;
using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.Swagger.AuthGate.Controllers;
using Fanex.NCB.Swagger.AuthGate.Services;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Fanex.NCB.Swagger.AuthGate.UnitTests.Controllers
{
    public class UserResourceControllerTests
    {
        private readonly Fixture _fixture;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IMenuService> _menuService;
        private readonly UserResourceController _userResourceController;

        public UserResourceControllerTests()
        {
            _fixture = new Fixture();
            _userService = new Mock<IUserService>();
            _menuService = new Mock<IMenuService>();
            _userResourceController = new UserResourceController(_userService.Object, _menuService.Object);
        }

        [Fact]
        public void QueryMenu_WhenCallServices_WithCorrectParam()
        {
            //Arrange
            var id = _fixture.Create<int>();
            var siteId = _fixture.Create<int>();
            var env = _fixture.Create<string>();
            var menuItems = GetMenuItems(5);

            _userService.Setup(_ => _.GetMenuItem(id, siteId, env, true)).Returns(menuItems);
            IEnumerable<MenuItem> actualParam = null;
            _menuService.Setup(_ => _.GetMenus(It.IsAny<IEnumerable<MenuItem>>()))
                .Callback<IEnumerable<MenuItem>>(param => actualParam = param);

            //Act
            _userResourceController.Query(id, siteId, env);

            //Assert
            Assert.Equal(menuItems, actualParam);
        }

        [Fact]
        public void QueryMenu_WhenCall_ReturnCorrectData()
        {
            //Arrange
            var menu = GetMenuDTO(10).ToList();
            _menuService.Setup(_ => _.GetMenus(It.IsAny<IEnumerable<MenuItem>>())).Returns(menu);

            //Act
            var actual = _userResourceController.Query(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()) as List<Menu>;

            //Assert
            Assert.Equal(menu, actual);
        }

        private IEnumerable<MenuItem> GetMenuItems(int size)
        {
            var menuItems = new List<MenuItem>();
            for (int i = 0; i < size; i++)
            {
                menuItems.Add(new MenuItem { Id = ++i });
            }

            return menuItems;
        }

        private IEnumerable<Menu> GetMenuDTO(int size)
        {
            var menuItems = new List<Menu>();
            for (int i = 0; i < size; i++)
            {
                menuItems.Add(new Menu { Id = ++i });
            }

            return menuItems;
        }
    }
}