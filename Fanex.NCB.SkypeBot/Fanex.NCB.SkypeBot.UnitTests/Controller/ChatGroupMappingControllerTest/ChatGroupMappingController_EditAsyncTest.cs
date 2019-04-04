using AutoFixture;
using Fanex.NCB.Common.DTO;
using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Model.Criteria.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using Fanex.NCB.SkypeBot.Services.Setting;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.ChatGroupMappingControllerTest
{
    public class ChatGroupMappingController_EditAsyncTest
    {
        [Fact]
        public async Task EditAsync_WhenCalled_UpdateChatGroupMappingAsync()
        {
            // Arrange
            var fixture = new Fixture();
            var chatGroupMappingService = new Mock<IChatGroupMappingService>();
            var settingService = new Mock<ISettingService>();
            var mappingStatusService = new Mock<IMappingStatusService>();
            var controller = new ChatGroupMappingController(chatGroupMappingService.Object, settingService.Object, mappingStatusService.Object);

            var criteria = fixture.Create<EditChatGroupMappingCriteria>();
            var resultModel = fixture.Create<ResultModel>();
            chatGroupMappingService.Setup(s => s.UpdateChatGroupMappingAsync(criteria)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.EditAsync(criteria) as JsonResult;

            // Assert
            result.Value.Should().BeEquivalentTo(resultModel);
        }
    }
}
