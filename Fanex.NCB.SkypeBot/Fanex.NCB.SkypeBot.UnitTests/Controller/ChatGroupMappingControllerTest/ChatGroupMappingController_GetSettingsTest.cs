using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.Base.Models;
using Fanex.NCB.Services.Interfaces;
using Fanex.NCB.SkypeBot.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.ChatGroupMappingControllerTest
{
    public class ChatGroupMappingController_GetSettingsTest
    {
        [Fact]
        public async Task GetSettingsAsync_WhenCalled_GetDataFromServices()
        {
            // Arrange
            var fixture = new Fixture();
            var chatGroupMappingService = new Mock<IChatGroupMappingService>();
            var settingService = new Mock<ISettingService>();
            var controller = new ChatGroupMappingController(chatGroupMappingService.Object, settingService.Object);

            var botNames = fixture.CreateMany<Models.Setting.BotName>(2);
            var mappingStatuses = fixture.CreateMany<BaseModel>(2).ToList();
            var recipientTypes = fixture.CreateMany<BaseModel>(2);
            var recipients = fixture.CreateMany<Models.Setting.Recipient>(2);

            settingService.Setup(s => s.GetBotNameAsync()).Returns(Task.FromResult(botNames));
            settingService.Setup(s => s.GetAllMappingStatus()).Returns(mappingStatuses);
            settingService.Setup(s => s.GetRecipientTypesAsync()).Returns(Task.FromResult(recipientTypes));
            settingService.Setup(s => s.GetRecipientsAsync()).Returns(Task.FromResult(recipients));

            var expectedResult = new { botNames, recipientTypes, mappingStatuses, recipients };

            // Act
            var result = await controller.GetSettings() as JsonResult;

            // Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}