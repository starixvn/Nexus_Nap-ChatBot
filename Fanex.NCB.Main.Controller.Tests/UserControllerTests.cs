namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Models;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class UserControllerTests
    {
        private ISiteService siteService;
        private INapSession napSession;
        private INapSettings napSettings;
        private ISecurityService securityService;

        [Fact]
        public void GetMenu_WhenCalled_ReturnExpectedJson()
        {
            var controller = GetController();

            var expected = new List<Menu>
            {
                new Menu {Id = 1, Name = "Test", Url = "/Test" },
                new Menu { Id = 2, Name = "Sub", Url = "Sub"}
            };
            var menuItem = new List<MenuItem>
            {
                new MenuItem {Id = 1, Name = "Test", Url = "/Test"},
                new MenuItem {Id = 2, Name = "Sub", Url = "Sub" }
            };
            napSession.MenuItemList = menuItem;

            //Act
            var actual = controller.GetMenu();

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<IEnumerable<Menu>>(jsonResult.Value);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task CheckUserOnlineStatus_WhenSucessReCheckIn_ReturnExpectedJsonResult()
        {
            var controller = GetController();
            napSession.OnUserId = 8;
            var expected = new { OnUserId = 8, ErrCode = (int)RecheckInStatus.Success };
            //Actual
            var actual = await controller.CheckUserOnlineStatus();

            //Assert

            var json = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<object>(json.Value);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task CheckUserOnlineStatus_WhenInvalidSite_ReturnExpectedJsonResult()
        {
            var controller = GetController();
            napSettings.EnableReCheckIn.Returns(true);
            napSession.OnUserId = 8;
            siteService.IsActivatedAsync(Arg.Any<int>()).Returns(false);
            var expected = new { OnUserId = 8, ErrCode = (int)RecheckInStatus.InvalidSite };
            //Actual
            var actual = await controller.CheckUserOnlineStatus();

            //Assert

            var json = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<object>(json.Value);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task CheckUserOnlineStatus_WhenKickOut_ReturnExpectedJsonResult()
        {
            var controller = GetController();
            napSettings.EnableReCheckIn.Returns(true);
            napSession.OnUserId = 8;
            siteService.IsActivatedAsync(Arg.Any<int>()).Returns(true);
            securityService.ReCheckInAsync(napSession.OnUserId, Arg.Any<int>()).Returns(2);
            var expected = new { OnUserId = 8, ErrCode = (int)RecheckInStatus.KickOut };
            //Actual
            var actual = await controller.CheckUserOnlineStatus();

            //Assert

            var json = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<object>(json.Value);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public async Task CheckUserOnlineStatus_SessionUserIdIsZero_ReturnKickOutJsonResult()
        {
            var controller = GetController();
            napSession = null;
            var expected = new { OnUserId = 0, ErrCode = (int)RecheckInStatus.KickOut };
            //Actual
            var actual = await controller.CheckUserOnlineStatus();

            //Assert

            var json = Assert.IsType<JsonResult>(actual);
            var result = Assert.IsAssignableFrom<object>(json.Value);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(result));
        }

        private UserController GetController()
        {
            siteService = Substitute.For<ISiteService>();
            napSettings = Substitute.For<INapSettings>();
            securityService = Substitute.For<ISecurityService>();
            napSession = Substitute.For<INapSession>();

            return new UserController(securityService, siteService, napSession, napSettings);
        }
    }
}