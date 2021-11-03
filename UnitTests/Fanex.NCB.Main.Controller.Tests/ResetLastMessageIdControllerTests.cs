namespace Fanex.NCB.Main.Controllers.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;

    public class ResetLastMessageIdControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private INapSettings napSettings;
        private INapSession napSession;
        private IUserService userService;
        private IResetLastMessageIdService resetLastMessageIdService;

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            var controller = GetController();
            userService.IsHavingPermission(napSession.UserId, napSettings.SiteId, FunctionKey.ResetLastMessageId, Policy.Host)
                .Returns(true);

            // Action
            var actual = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var haveHostPermission = Assert.IsType<bool>(viewResult.ViewData["HaveHostPermission"]);
            Assert.True(haveHostPermission);
        }

        [Fact]
        public async Task GetCurrentPreviousMaxMessage_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var currentMaxMessageInfos = fixture.Create<IEnumerable<CurrentMaxMessageInfo>>();
            var messageTypeInfos = fixture.Create<IEnumerable<MessageTypeInfo>>();
            var controller = GetController();
            resetLastMessageIdService.GetCurrentPreviousMaxMessageAsync().Returns((currentMaxMessageInfos,messageTypeInfos));

            // Act
            var result = await controller.GetCurrentPreviousMaxMessage();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var currentMaxMessage = (IEnumerable<CurrentMaxMessageInfo>)jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value);
            var messageType = (IEnumerable<MessageTypeInfo>)jsonResult.Value.GetType().GetProperty("messageTypeInfos").GetValue(jsonResult.Value);
            Assert.Equal(currentMaxMessageInfos, currentMaxMessage);
            Assert.Equal(messageTypeInfos, messageType);
        }

        [Fact]
        public async Task GetNewPreviousMaxMessage_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetNewPreviousMaxMessageCriteria>();
            var messagePostTimeInfos = fixture.Create<IEnumerable<MessagePostTime>>();
            var newMaxMessageId = fixture.Create<int>();
            var controller = GetController();
            resetLastMessageIdService.GetNewPreviousMaxMessageAsync(criteria).Returns((messagePostTimeInfos, newMaxMessageId));

            // Act
            var result = await controller.GetNewPreviousMaxMessage(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var messagePostTime = (IEnumerable<MessagePostTime>)jsonResult.Value.GetType().GetProperty("messagePostTimeInfos").GetValue(jsonResult.Value);
            var newId = jsonResult.Value.GetType().GetProperty("newMaxMessageId").GetValue(jsonResult.Value);
            Assert.Equal(messagePostTimeInfos, messagePostTime);
            Assert.Equal(newMaxMessageId, newId);
        }

        [Fact]
        public async Task GetMissingMessageCsd_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetMissingMesssageCsdCriteria>();
            var missingMessageCsd = fixture.Create<IEnumerable<MessageRequestId>>();
            var controller = GetController();
            resetLastMessageIdService.GetMissingMessageCsdAsync(criteria).Returns(missingMessageCsd);

            // Act
            var result = await controller.GetMissingMessageCsd(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<MessageRequestId>>(jsonResult.Value);
            Assert.Equal(missingMessageCsd, actual);
        }

        [Fact]
        public async Task GetMissingMessageInfo_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var criteria = fixture.Create<GetMissingMessageInfoCriteria>();
            var missingMessageInfos = fixture.Create<IEnumerable<MissingMesssageInfo>>();
            var controller = GetController();
            resetLastMessageIdService.GetMissingMessageInfoAsync(criteria).Returns(missingMessageInfos);

            // Act
            var result = await controller.GetMissingMessageInfo(criteria);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<IEnumerable<MissingMesssageInfo>>(jsonResult.Value);
            Assert.Equal(missingMessageInfos, actual);
        }

        [Fact]
        public async Task CreateMissingMessage_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var command = fixture.Create<CreateMissingMessageCommand>();
            var commandResult = fixture.Create<CommandResult>();
            var controller = GetController();
            resetLastMessageIdService.CreateMissingMessageAsync(command).Returns(commandResult);

            // Act
            var result = await controller.CreateMissingMessage(command);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task EditMessageInfo_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var command = fixture.Create<EditMessageInfoCommand>();
            var commandResult = fixture.Create<CommandResult>();
            var controller = GetController();
            resetLastMessageIdService.EditMessageInfoAsync(command).Returns(commandResult);

            // Act
            var result = await controller.EditMessageInfo(command);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var actual = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.Equal(commandResult, actual);
        }

        private ResetLastMessageIdController GetController()
        {
            napSettings = Substitute.For<INapSettings>();
            napSession = Substitute.For<INapSession>();
            userService = Substitute.For<IUserService>();
            resetLastMessageIdService = Substitute.For<IResetLastMessageIdService>();

            return new ResetLastMessageIdController(napSettings, napSession, userService, resetLastMessageIdService);
        }
    }
}
