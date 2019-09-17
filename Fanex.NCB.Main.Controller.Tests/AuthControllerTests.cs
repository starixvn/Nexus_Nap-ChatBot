namespace Fanex.NCB.Main.Controllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Core.Resources;
    using Fanex.NCB.Models.Auth;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Routing;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class AuthControllerTests
    {
        private IUserService userService;
        private IGroupService groupService;
        private ISiteService siteService;
        private INapSession napSession;
        private INapSettings napSettings;
        private ISecurityService securityService;
        private ISyncDataService syncDataService;

        [Fact]
        public void Index_WhenCalled_ReturnView()
        {
            var controller = GetController();
            var returnUrl = "/Test";
            bool inValidLogin = false;
            //Actual
            IActionResult actual = controller.Index(returnUrl, inValidLogin);

            //Assert
            Assert.IsType<ViewResult>(actual);
        }

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
        public async Task Logout_WhenCalled_ExecutedExpectedMethodsAndReturnViewResult()
        {
            var controller = GetControllerForLogoutMethod();
            //Actual
            var actual = await controller.Logout();

            //Assert
            await securityService.Received().LogoutAsync(Arg.Any<int>());
            napSession.Received().ClearAll();
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", viewResult.ActionName);
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

        #region SSO Login Tests

        [Fact]
        public async Task SSO_IsNotValidTime_RedirectToAction()
        {
            var controller = GetController();
            var sso = new SsoLogin
            {
                OnUserId = Cryptography.EncryptPassword("8"),
                UserId = Cryptography.EncryptPassword("8"),
                Time = Cryptography.EncryptPassword(DateTime.Now.ToString()),
                RedirectUrl = "/Test"
            };
            securityService.GetTimeAsync().Returns(DateTime.Now.AddYears(1).ToString());
            //Actual
            var actual = await controller.SSO(sso);

            //Assert
            var result = Assert.IsType<RedirectResult>(actual);
            Assert.Equal("~/Auth/Index", result.Url);
        }

        [Fact]
        public async Task SSO_IsValidTimeAndCannotSSOLogin_RedirectToAction()
        {
            var controller = GetController();
            var sso = new SsoLogin
            {
                OnUserId = Cryptography.EncryptPassword("8"),
                UserId = Cryptography.EncryptPassword("8"),
                Time = Cryptography.EncryptPassword(DateTime.Now.ToString()),
                RedirectUrl = "/Test"
            };
            securityService.GetTimeAsync().Returns(DateTime.Now.AddYears(-1).ToString());
            securityService.SSOLoginAsync(Arg.Any<SSOLoginParameter>()).Returns(0);
            //Actual
            var actual = await controller.SSO(sso);

            //Assert
            var result = Assert.IsType<RedirectResult>(actual);
            Assert.Equal("~/Auth/Index", result.Url);
        }

        [Fact]
        public async Task SSO_IsValidTimeAndSSOLoginSuccess_ButNoUser_RedirectAction()
        {
            var controller = GetController();
            var sso = new SsoLogin
            {
                OnUserId = Cryptography.EncryptPassword("8"),
                UserId = Cryptography.EncryptPassword("8"),
                Time = Cryptography.EncryptPassword(DateTime.Now.ToString()),
                RedirectUrl = "/Test"
            };
            securityService.GetTimeAsync().Returns(DateTime.Now.AddYears(-1).ToString());
            securityService.SSOLoginAsync(Arg.Any<SSOLoginParameter>()).Returns(8);
            //Actual
            var actual = await controller.SSO(sso);

            //Assert
            var result = Assert.IsType<RedirectResult>(actual);
            Assert.Equal("~/Auth/Index", result.Url);
        }

        [Fact]
        public async Task SSO_IsValidTimeAndSSOLoginSuccess_GetUserSuccess_RedirectAction()
        {
            var controller = FakeDoCheckInController();
            var sso = new SsoLogin
            {
                OnUserId = Cryptography.EncryptPassword("8"),
                UserId = Cryptography.EncryptPassword("8"),
                Time = Cryptography.EncryptPassword(DateTime.Now.ToString()),
                RedirectUrl = "/Test"
            };
            var user = new User
            {
                Id = 8,
                Name = "sa",
                Username = "sa",
            };
            securityService.SSOLoginAsync(Arg.Any<SSOLoginParameter>()).Returns(8);
            userService.GetAsync(Arg.Any<int>()).Returns(user);
            securityService.GetTimeAsync().Returns(DateTime.Now.AddYears(-1).ToString());
            var menuItems = new List<MenuItem>
            {
                new MenuItem {Id = 1, Name = "Test", Url = "/Test"},
                new MenuItem {Id = 2, Name = "Sub", Url = "Sub" }
            };
            userService.GetMenuItemAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), true).Returns(menuItems);

            //Actual
            var actual = await controller.SSO(sso);

            //Assert
            var result = Assert.IsType<RedirectResult>(actual);
            Assert.Equal("/Test", result.Url);
        }

        #endregion SSO Login Tests

        #region SignIn tests

        [Fact]
        public async Task SignIn_WhenNoUser_ReturnExpectedResult()
        {
            var controller = GetController();
            var userLogin = new UserLogin();
            var expected = new RouteValueDictionary();
            expected.Add("ReturnUrl", userLogin.ReturnUrl);
            expected.Add("invalidLogin", true);
            expected.Add("errorMsg", Labels.UserNotExists);

            //Actual
            var actual = await controller.SignIn(new UserLogin());

            //Assert
            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(expected, result.RouteValues);
        }

        [Fact]
        public async Task SignIn_WrongPassword_ReturnExpectedResult()
        {
            var controller = GetController();
            var userLogin = new UserLogin
            {
                Username = "sa",
                Password = "1234"
            };
            var expected = new RouteValueDictionary();
            expected.Add("ReturnUrl", userLogin.ReturnUrl);
            expected.Add("invalidLogin", true);
            expected.Add("errorMsg", Labels.WrongPassword);
            var user = new User
            {
                Username = "sa",
                Password = "Test"
            };
            userService.GetAsync(Arg.Any<string>()).Returns(user);

            //Actual
            var actual = await controller.SignIn(userLogin);

            //Assert
            var result = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(expected, result.RouteValues);
        }

        [Fact]
        public async Task SignIn_NoPermission_ReturnExpectedResult()
        {
            var controller = GetController();
            var userLogin = new UserLogin
            {
                Username = "sa",
                Password = "1234"
            };
            var expected = new RouteValueDictionary();
            expected.Add("ReturnUrl", userLogin.ReturnUrl);
            expected.Add("invalidLogin", true);
            expected.Add("errorMsg", Labels.NoPermision);
            var user = new User
            {
                Username = "sa",
                Password = NAP.Client.Helpers.Common.Cryptography.CfsEnCode("1234"),
                Group = new Group { Id = 2, Name = "Test" }
            };

            userService.GetAsync(Arg.Any<string>()).Returns(user);
            groupService.GetAsync(Arg.Any<int>()).Returns(new Group { Id = 2, Name = "Test", IsActive = true });
            //Actual
            var actual = await controller.SignIn(userLogin);

            //Assert
            var result = Assert.IsType<RedirectToActionResult>(actual);

            Assert.Equal("Index", result.ActionName);
            Assert.Equal(expected, result.RouteValues);
        }

        [Fact]
        public async Task SignIn_Valid_ReturnExpectedResult()
        {
            var controller = FakeDoCheckInController();
            var userLogin = new UserLogin
            {
                Username = "sa",
                Password = "1234",
                ReturnUrl = "/Test"
            };
            var expected = new RouteValueDictionary();
            expected.Add("ReturnUrl", userLogin.ReturnUrl);
            expected.Add("invalidLogin", true);
            expected.Add("errorMsg", Labels.NoPermision);
            var user = new User
            {
                Username = "sa",
                Password = NAP.Client.Helpers.Common.Cryptography.CfsEnCode("1234"),
                Group = new Group { Id = 2, Name = "Test" }
            };
            var siteLevel = new List<SiteLevel>
            {
                new SiteLevel { Site = new Site { Id = 69}, IsActive = true }
            };
            napSettings.SiteId.Returns(69);
            userService.GetAsync(Arg.Any<string>()).Returns(user);
            groupService.GetAsync(Arg.Any<int>())
                .Returns(new Group { Id = 2, Name = "Test", IsActive = true });
            userService.GetSiteLevelAsync(Arg.Any<int>(), Arg.Any<string>())
                   .Returns(siteLevel);
            //Actual
            var actual = await controller.SignIn(userLogin);

            //Assert
            Assert.IsType<RedirectResult>(actual);
        }

        #endregion SignIn tests

        #region helper

        private AuthController FakeDoCheckInController()
        {
            var authService = Substitute.For<IAuthenticationService>();
            var urlHelper = Substitute.For<IUrlHelper>();
            var urlHelperFactory = Substitute.For<IUrlHelperFactory>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var session = Substitute.For<ISession>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);
            serviceProvider.GetService(typeof(IUrlHelper)).Returns(urlHelper);
            serviceProvider.GetService(typeof(IUrlHelperFactory)).Returns(urlHelperFactory);
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider,
                    Session = session
                }
            };

            var controller = GetController();
            controller.ControllerContext = controllerContext;
            controller.ControllerContext = controllerContext;
            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Any;
            controller.ControllerContext.HttpContext.Request.Headers["User-Agent"] = "12345";
            controller.Url.IsLocalUrl(Arg.Any<string>()).Returns(true);
            napSession.MenuItemList = new List<MenuItem>
            {
                new MenuItem {Id = 1, Name = "Test", Url = "/Test"},
                new MenuItem {Id = 2, Name = "Sub", Url = "Sub" }
            };
            return controller;
        }

        private AuthController GetControllerForLogoutMethod()
        {
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider
                }
            };

            var controller = GetController();
            controller.ControllerContext = controllerContext;
            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Any;
            controller.ControllerContext.HttpContext.Request.Headers["User-Agent"] = "12345";
            controller.Url = Substitute.For<IUrlHelper>();
            return controller;
        }

        private AuthController GetController()
        {
            userService = Substitute.For<IUserService>();
            groupService = Substitute.For<IGroupService>();
            siteService = Substitute.For<ISiteService>();
            napSettings = Substitute.For<INapSettings>();
            securityService = Substitute.For<ISecurityService>();
            syncDataService = Substitute.For<ISyncDataService>();
            napSession = Substitute.For<INapSession>();

            return new AuthController(userService, groupService, securityService, siteService, napSession, napSettings, syncDataService);
        }

        #endregion helper
    }
}