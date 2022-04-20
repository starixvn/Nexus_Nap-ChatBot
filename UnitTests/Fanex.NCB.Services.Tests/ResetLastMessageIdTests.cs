namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using Xunit;

    public class ResetLastMessageIdTests
    {
        private readonly IFixture fixture = new Fixture();
        private IBaseService baseService;
        private IConfiguration configuration;

        [Fact]
        public async Task GetCurrentPreviousMaxMessageAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var criteria = fixture.Create<GetCurrentPreviousMaxMessageCriteria>();
            var currentMaxMessageInfos = fixture.Create<IEnumerable<CurrentMaxMessageInfo>>();
            var messageTypeInfos = fixture.Create<IEnumerable<MessageTypeInfo>>();
            var tuple = (currentMaxMessageInfos, messageTypeInfos);

            baseService.FetchMultipleAsync<CurrentMaxMessageInfo, MessageTypeInfo>(Arg.Do<GetCurrentPreviousMaxMessageCriteria>(x => criteria = x)).Returns(tuple);

            //Act
            var actual = await service.GetCurrentPreviousMaxMessageAsync();

            //Assert
            Assert.Equal(tuple, actual);
        }

        [Fact]
        public async Task GetNewPreviousMaxMessageAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var criteria = fixture.Create<GetNewPreviousMaxMessageCriteria>();
            var messagePostTimeInfos = fixture.Create<IEnumerable<MessagePostTime>>();
            var newMaxMessageId = fixture.Create<IEnumerable<int>>();
            var tuple = (messagePostTimeInfos, newMaxMessageId);

            baseService.FetchMultipleAsync<MessagePostTime, int>(Arg.Do<GetNewPreviousMaxMessageCriteria>(x => criteria = x)).Returns(tuple);

            //Act
            var actual = await service.GetNewPreviousMaxMessageAsync(criteria);

            //Assert
            Assert.Equal(messagePostTimeInfos, actual.Item1);
            Assert.Equal(newMaxMessageId.FirstOrDefault(), actual.Item2);
        }

        [Fact]
        public async Task GetMissingMessageCsdAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var criteria = fixture.Create<GetMissingMesssageCsdCriteria>();
            var missingMessageCsd = fixture.Create<IEnumerable<MessageRequestId>>();

            baseService.GetDataAsync<MessageRequestId>(Arg.Do<GetMissingMesssageCsdCriteria>(x => criteria = x)).Returns(missingMessageCsd);

            //Act
            var actual = await service.GetMissingMessageCsdAsync(criteria);

            //Assert
            Assert.Equal(missingMessageCsd, actual);
        }

        [Fact]
        public async Task GetMissingMessageInfoAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var criteria = fixture.Create<GetMissingMessageInfoCriteria>();
            var missingMessageInfos = fixture.Create<IEnumerable<MissingMesssageInfo>>();

            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Do<GetMissingMessageInfoCriteria>(x => criteria = x)).Returns(missingMessageInfos);

            //Act
            var actual = await service.GetMissingMessageInfoAsync(criteria);

            //Assert
            Assert.Equal(missingMessageInfos, actual);
        }

        [Fact]
        public async Task GetMissingProblemAccountAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var criteria = fixture.Create<GetMissingProblemAccountCriteria>();
            var fromToLogId = fixture.Create<IEnumerable<FromToLogId>>();
            var missingMesssageInfo = fixture.Create<IEnumerable<MissingMesssageInfo>>();
            var tuple = (fromToLogId, missingMesssageInfo);

            baseService.FetchMultipleAsync<FromToLogId, MissingMesssageInfo>(Arg.Do<GetMissingProblemAccountCriteria>(x => criteria = x)).Returns(tuple);

            //Act
            var actual = await service.GetMissingProblemAccountAsync(criteria);

            //Assert
            Assert.Equal(tuple, actual);
        }

        [Fact]
        public async Task CreateMissingMessageAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var command = fixture.Create<CreateMissingMessageCommand>();
            var commandResult = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(Arg.Do<CreateMissingMessageCommand>(x => command = x)).Returns(commandResult);

            //Act
            var actual = await service.CreateMissingMessageAsync(command);

            //Assert
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task EditMessageInfoAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var command = fixture.Create<EditMessageInfoCommand>();
            var commandResult = fixture.Create<CommandResult>();

            baseService.ExecuteNonQueryAsync(Arg.Do<EditMessageInfoCommand>(x => command = x)).Returns(commandResult);

            //Act
            var actual = await service.EditMessageInfoAsync(command);

            //Assert
            Assert.Equal(commandResult, actual);
        }

        private ResetLastMessageIdService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            configuration = Substitute.For<IConfiguration>();

            return new ResetLastMessageIdService(baseService, configuration);
        }
    }
}
