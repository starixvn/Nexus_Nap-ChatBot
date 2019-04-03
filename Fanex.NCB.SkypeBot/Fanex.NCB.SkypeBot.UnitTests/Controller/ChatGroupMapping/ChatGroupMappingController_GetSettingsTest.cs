using AutoFixture;
using Fanex.NCB.SkypeBot.Controllers;
using Fanex.NCB.SkypeBot.Model.DTO.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Model.DTO.NotificationType;
using Fanex.NCB.SkypeBot.Services.ChatGroupMapping;
using Fanex.NCB.SkypeBot.Services.MappingStatusService;
using Fanex.NCB.SkypeBot.Services.Setting;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.NCB.SkypeBot.UnitTests.Controller.ChatGroupMapping
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
            var mappingStatusService = new Mock<IMappingStatusService>();
            var controller = new ChatGroupMappingController(chatGroupMappingService.Object, settingService.Object, mappingStatusService.Object);

            var botNames = fixture.CreateMany<BotName>(2).ToList();
            var mappingStatuses = fixture.CreateMany<MappingStatus>(2).ToList();
            var recipientTypes = fixture.CreateMany<RecipientType>(2).ToList();            
            var recipients = fixture.CreateMany<Recipient>(2).ToList();

            settingService.Setup(s => s.GetBotNameAsync()).Returns(Task.FromResult(botNames));
            mappingStatusService.Setup(s => s.GetMappingStatus()).Returns(mappingStatuses);
            settingService.Setup(s => s.GetRecipientTypesAsync()).Returns(Task.FromResult(recipientTypes));
            settingService.Setup(s => s.GetRecipientsAsync()).Returns(Task.FromResult(recipients));

            var expectedResult = new { botNames, recipientTypes, mappingStatuses, recipients };

            // Act
            var result = await controller.GetSettingsAsync() as JsonResult;

            // Assert
            result.Value.Should().BeEquivalentTo(expectedResult);
        }
    }
}
