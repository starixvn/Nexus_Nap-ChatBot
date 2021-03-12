using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Fanex.NCB.Main.Controllers.Tests
{
    public class ErrorControllerTests
    {
        private readonly ErrorController controller;

        public ErrorControllerTests()
        {
            controller = new ErrorController();
        }

        [Fact]
        public void PageNotFound_WhenCalled_ReturnViewResult()
        {
            var result = controller.PageNotFound();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void UnauthorizedUser_WhenCalled_ReturnViewResult()
        {
            var result = controller.UnauthorizedUser();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void InternalServerError_WhenCalled_ReturnViewResult()
        {
            var result = controller.InternalServerError();
            Assert.IsType<ViewResult>(result);
        }
    }
}