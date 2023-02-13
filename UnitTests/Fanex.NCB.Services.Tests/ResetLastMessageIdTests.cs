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
    using Newtonsoft.Json;
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
            var currentMaxMessageInfos = fixture.Create<IEnumerable<CurrentMaxMessageInfo>>();
            var messageTypeInfos = fixture.Create<IEnumerable<MessageTypeInfo>>();
            var tuple = (currentMaxMessageInfos, messageTypeInfos);

            baseService.FetchMultipleAsync<CurrentMaxMessageInfo, MessageTypeInfo>(Arg.Any<GetCurrentPreviousMaxMessageCriteria>()).Returns(tuple);

            //Act
            var actual = await service.GetCurrentPreviousMaxMessageAsync();

            //Assert
            Assert.Equal(tuple, actual);
        }

        [Fact]
        public async Task GetMissingMessageInfoAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var missingMessageInfos = new List<MissingMesssageInfo> {
                new MissingMesssageInfo { JobIdentify = 1, VoidingHistoryCollectionId = 1, Content = "ABC", MessageId = 300 },
                new MissingMesssageInfo { JobIdentify = 2, VoidingHistoryCollectionId = 2000, Content = "ABC", MessageId = 2 },
            };

            var missedProblemAccounts = new List<MissingMesssageInfo>();
            var missedMonitoredPlayers = new List<MissingMesssageInfo>
            {
               new MissingMesssageInfo { JobIdentify = 5, VoidingHistoryCollectionId = 5, Content = "ABC", MessageId = 5 }
            };

            var msgList = new List<CurrentMaxMessageInfo>
            {
                new CurrentMaxMessageInfo {JobIdentify = 1, ReferenceId = 1},
                new CurrentMaxMessageInfo {JobIdentify = 2, ReferenceId = 1000},
                new CurrentMaxMessageInfo {JobIdentify = 4, ReferenceId = 1000},
                new CurrentMaxMessageInfo {JobIdentify = 5, ReferenceId = 1000}
            };

            var expected = new List<MissingMesssageInfo> {
                new MissingMesssageInfo { JobIdentify = 1, VoidingHistoryCollectionId = 1, Content = "ABC", MessageId = 300 },
                new MissingMesssageInfo { JobIdentify = 2, VoidingHistoryCollectionId = 2000, Content = "ABC", MessageId = 2 },
                new MissingMesssageInfo { JobIdentify = 5, VoidingHistoryCollectionId = 5, Content = "ABC", MessageId = 5 }
            };

            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingMessageInfoCriteria>()).Returns(missingMessageInfos);
            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingProblemAccountCriteria>()).Returns(missedProblemAccounts);
            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingMonitoredPlayerCriteria>()).Returns(missedMonitoredPlayers);
            var contentTypeList = fixture.Create<IEnumerable<MessageTypeInfo>>();
            var abnormalMissingMsgList = fixture.Create<IEnumerable<MessageRequestId>>();
            //Act
            var actual = await service.GetMissingMessageInfoAsync(msgList, contentTypeList, abnormalMissingMsgList);

            //Assert
            Assert.Equal(300, msgList.FirstOrDefault(x => x.JobIdentify == 1).MaxReferenceId);
            Assert.Equal(2000, msgList.FirstOrDefault(x => x.JobIdentify == 2).MaxReferenceId);
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        [Fact]
        public async Task CreateMissingMessageAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var commandResult = new CommandResult { Code = 0, Message = string.Empty };
            var commandResult2 = new CommandResult { Code = 1, Message = "ABCD" };
            var expected = new CommandResult { Code = 1, Message = "ABCD" };
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateMissingMessageCommand>()).Returns(commandResult);
            baseService.ExecuteNonQueryAsync(Arg.Any<EditMessageInfoCommand>()).Returns(commandResult2);

            //Act
            var param = new List<MissingMesssageInfo> { new MissingMesssageInfo { MessageId = 1, JobIdentify = 1 } };
            var actual = await service.CreateMissingMessageAsync(param);

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        private ResetLastMessageIdService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            configuration = Substitute.For<IConfiguration>();

            return new ResetLastMessageIdService(baseService, configuration);
        }
    }
}