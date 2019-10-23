namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System.Net;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Xunit;

    public class HomeControllerTests
    {
        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewResult()
        {
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Connection.LocalIpAddress = IPAddress.Parse("10.10.10.10");
            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("10.10.10.11");

            //Actual
            var actual = controller.Index();

            //Assert
            var result = actual as ViewResult;
            Assert.Equal("Index", result.ViewName);
        }

        [Fact]
        public void Index_WhenCalled_ReturnExpectedViewData()
        {
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Connection.LocalIpAddress = IPAddress.Parse("10.10.10.10");
            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("10.10.10.11");

            //Actual
            var actual = controller.Index();

            //Assert
            var result = actual as ViewResult;

            Assert.Equal("10.10.10.10", result.ViewData["LocalIpAddress"]);
            Assert.Equal("10.10.10.11", result.ViewData["RemoteIpAddress"]);
        }
    }
}
