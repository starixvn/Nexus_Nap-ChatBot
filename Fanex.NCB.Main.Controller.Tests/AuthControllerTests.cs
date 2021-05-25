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
    using Fanex.NCB.Core.Resources;
    using Fanex.NCB.Models.Auth;
    using Fanex.NCB.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class AuthControllerTests
    {
        private IUserService userService;
        private IGroupService groupService;
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
        public async Task Logout_WhenCalled_ExecutedExpectedMethodsAndReturnViewResult()
        {
            var controller = FakeDoCheckInController();
            //Actual
            var actual = await controller.Logout();

            //Assert
            await securityService.Received().LogoutAsync(Arg.Any<int>());
            napSession.Received().ClearAll();
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", viewResult.ActionName);
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
            var controller = FakeDoCheckInController();
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
            var controller = FakeDoCheckInController();
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
            var expected = new RouteValueDictionary
            {
                { "ReturnUrl", userLogin.ReturnUrl },
                { "invalidLogin", true },
                { "errorMsg", Labels.UserNotExists }
            };

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
            var expected = new RouteValueDictionary
            {
                { "ReturnUrl", userLogin.ReturnUrl },
                { "invalidLogin", true },
                { "errorMsg", Labels.WrongPassword }
            };
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
            var expected = new RouteValueDictionary
            {
                { "ReturnUrl", userLogin.ReturnUrl },
                { "invalidLogin", true },
                { "errorMsg", Labels.WrongPassword }
            };
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
                Password = NAP.Client.Helpers.Common.Cryptography.CfsEnCode("1234"),
                ReturnUrl = "/Test"
            };

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
            var result = Assert.IsType<RedirectResult>(actual);

            Assert.Equal("~/Test", result.Url);
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

        private AuthController GetController()
        {
            userService = Substitute.For<IUserService>();
            groupService = Substitute.For<IGroupService>();
            napSettings = Substitute.For<INapSettings>();
            securityService = Substitute.For<ISecurityService>();
            syncDataService = Substitute.For<ISyncDataService>();
            napSession = Substitute.For<INapSession>();

            return new AuthController(userService, groupService, securityService, napSession, napSettings, syncDataService);
        }

        #endregion helper
    }
}