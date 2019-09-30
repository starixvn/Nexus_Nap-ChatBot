namespace Fanex.NCB.AutoJobs.Tests.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Fanex.NCB.AutoJobs.Controllers;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class AuthControllerTests
    {
        private IOptionsSnapshot<IdentifyModel> auth;

        [Fact]
        public void Login_WhenCalled_ReturnExpectedView()
        {
            var controller = GetController();

            //Act
            var actual = controller.Login();

            //Expect
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public async Task Login_InvalidUsernamePassword_ReturnViewWithError()
        {
            var controller = GetController();

            //Act
            var actual = await controller.Login(string.Empty, "password");

            //Expect
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public async Task Login_ValidUsernamePassword_ReturnExpectedResult()
        {
            var controller = GetControllerForAuthenticate();

            //Act
            var actual = await controller.Login("sa", "1234aa");

            //Expect
            var result = Assert.IsType<RedirectResult>(actual);
            Assert.Equal("~/hangfire", result.Url);
        }

        [Fact]
        public async Task Logout_WhenCalled_ReturnExpectedResult()
        {
            var controller = GetControllerForAuthenticate();

            //Act
            var actual = await controller.Logout();

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", viewResult.ActionName);
        }

        private AuthController GetController()
        {
            auth = Substitute.For<IOptionsSnapshot<IdentifyModel>>();
            return new AuthController(auth);
        }

        private AuthController GetControllerForAuthenticate()
        {
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            var urlHelper = Substitute.For<IUrlHelper>();
            var urlHelperFactory = Substitute.For<IUrlHelperFactory>();
            var session = Substitute.For<ISession>();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider,
                    Session = session
                }
            };
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);
            serviceProvider.GetService(typeof(IUrlHelper)).Returns(urlHelper);
            serviceProvider.GetService(typeof(IUrlHelperFactory)).Returns(urlHelperFactory);

            auth = Substitute.For<IOptionsSnapshot<IdentifyModel>>();
            var authModel = new IdentifyModel
            {
                UserName = "sa",
                Password = Cryptography.EncryptPassword("1234aa")
            };

            auth.Value.Returns(authModel);
            var controller = new AuthController(auth)
            {
                ControllerContext = controllerContext
            };

            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Any;
            controller.ControllerContext.HttpContext.Request.Headers["User-Agent"] = "12345";

            return controller;
        }
    }
}