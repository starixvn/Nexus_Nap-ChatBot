namespace Fanex.NCB.SkypeBot.UnitTests.Controller.ChatGroupMappingControllerTest
{
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.SkypeBot.Controllers;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using Xunit;

    public class ChatGroupMappingController_IndexTest
    {
        [Fact]
        public void Index_WhenCalled_ReturnCorrectView()
        {
            // Arrange
            var chatGroupMappingService = new Mock<IChatGroupMappingService>();
            var settingService = new Mock<ISettingService>();
            var napSession = new Mock<INapSession>();
            var controller = new ChatGroupMappingController(chatGroupMappingService.Object, settingService.Object, napSession.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.ViewName.Should().Be("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }
    }
}