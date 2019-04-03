using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using Fanex.NCB.SkypeBot.Services.Setting;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.ChatGroupMapping
{
    public class ChatGroupMappingController_IndexTest
    {
        [Fact]
        public void Index_WhenCalled_ReturnCorrectView()
        {
            // Arrange
            var chatGroupMappingService = new Mock<IChatGroupMappingService>();
            var settingService = new Mock<ISettingService>();
            var mappingStatusService = new Mock<IMappingStatusService>();
            var controller = new ChatGroupMappingController(chatGroupMappingService.Object, settingService.Object, mappingStatusService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            result.ViewName.Should().Be("~/Views/SkypeBot/ChatGroupMapping/Index.cshtml");
        }
    }
}
