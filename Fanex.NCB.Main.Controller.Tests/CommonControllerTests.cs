namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Models.Common;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.SkypeBot.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class CommonControllerTests
    {
        private ICommonService commonService;

        [Fact]
        public async Task GetNotificationTypes_WhenCalled_ReturnJsonResult()
        {
            var controller = GetController();
            var fixture = new Fixture();
            var expectedTypes = fixture.Create<IEnumerable<NotificationType>>();

            commonService.GetNotificationTypesAsync().Returns(expectedTypes);

            var result = await controller.GetNotificationTypes().ConfigureAwait(false);

            var json = Assert.IsType<JsonResult>(result);
            var returnedTypes = Assert.IsAssignableFrom<IEnumerable<NotificationType>>(json.Value);
            Assert.Equal(expectedTypes, returnedTypes);
        }

        private CommonController GetController()
        {
            commonService = Substitute.For<ICommonService>();

            return new CommonController(commonService);
        }
    }
}