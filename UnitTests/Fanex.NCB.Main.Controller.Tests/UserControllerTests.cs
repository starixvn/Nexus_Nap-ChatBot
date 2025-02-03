namespace Fanex.NCB.Main.Controllers.Tests;

using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using NAP.Client.Configuration;
using NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Services;
using Core.Enum;
using Fanex.NCB.Core.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        // Arrange
        var controller = GetController();
        var item1 = new MenuItem { Id = 1, Name = "Test", Url = "/Test" };
        var expected = new List<Menu>
        {
            new()
            {
                Id = 1, Name = "Test", Url = "/Test",
                SubMenus = new List<Menu> { new() { Id = 3, Name = "Menu Item", Url = "/MenuItem" } }
            },
            new() { Id = 2, Name = "Sub", Url = "Sub", SubMenus = new List<Menu>() }
        };

        var menuItem = new List<MenuItem>
        {
            item1,
            new() { Id = 2, Name = "Sub", Url = "Sub" },
            new() { Id = 3, Name = "Menu Item", Url = "/MenuItem", Parent = item1 }
        };
        napSession.MenuItemList = menuItem;

        // Act
        var actual = controller.GetMenu() as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CheckUserOnlineStatus_WhenSucessReCheckIn_ReturnExpectedJsonResult()
    {
        var controller = GetController();
        napSession.OnUserId = 8;
        var expected = new { OnUserId = 8, ErrCode = (int)RecheckInStatus.Success, IsChangePermission = false };
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
        var expected = new { OnUserId = 8, ErrCode = (int)RecheckInStatus.InvalidSite, IsChangePermission = false };
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
        var expected = new { OnUserId = 8, ErrCode = (int)RecheckInStatus.InvalidSite, IsChangePermission = false };
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
        var expected = new { OnUserId = 0, ErrCode = (int)RecheckInStatus.KickOut, IsChangePermission = false };
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

        var inMemorySettings = new Dictionary<string, string>
        {
            { "EnableCheckPermissionChanged", "false" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        return new UserController(securityService, siteService, napSession, napSettings, configuration);
    }
}