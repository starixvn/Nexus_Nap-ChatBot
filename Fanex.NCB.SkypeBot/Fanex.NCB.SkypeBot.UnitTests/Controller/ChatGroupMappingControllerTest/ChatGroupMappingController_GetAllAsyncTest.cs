using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.Models.ChatGroupMapping;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.ChatGroupMappingControllerTest
{
    public class ChatGroupMappingController_GetAllAsyncTest
    {
        [Fact]
        public async Task GetAllAsync_WhenCalled_GetChatGroupMappingListAsync()
        {
            // Arrange
            var fixture = new Fixture();
            var chatGroupMappingService = new Mock<IChatGroupMappingService>();
            var settingService = new Mock<ISettingService>();
            var controller = new ChatGroupMappingController(chatGroupMappingService.Object, settingService.Object);

            var model = fixture.Create<GetChapGroupMapping>();
            var resultModel = fixture.Create<ValueTuple<IEnumerable<ChatGroupMappingResult>, long>>();
            chatGroupMappingService.Setup(s => s.GetChatGroupMappingListAsync(model)).Returns(Task.FromResult(resultModel));

            // Act
            var result = await controller.GetAll(model) as JsonResult;

            // Assert
            result.Value.Should().BeEquivalentTo(resultModel);
        }
    }
}