namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.ResetLastMessageId;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Services.Interfaces;
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
        private ICommonService commonService;

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
        public async Task GetAbnormalBetsMissingMessageAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var messageRequestId = fixture.Create<IEnumerable<MessageRequestId>>();
            var messageInfoList = fixture.Create<IEnumerable<CurrentMaxMessageInfo>>();

            baseService.GetDataAsync<MessageRequestId >(Arg.Any<GetMissingMesssageCsdCriteria>()).Returns(messageRequestId);

            //Act
            var actual = await service.GetAbnormalBetsMissingMessageAsync(messageInfoList);

            //Assert
            Assert.Equal(messageRequestId, actual);
        }

        [Fact]
        public async Task GetMissingMessageDataAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var missingMesssages = fixture.Create<IEnumerable<MissingMesssageInfo>>();
            var missingMesssageInfo = fixture.Create<IEnumerable<MissingMesssageInfo>>();

            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingMessageDataCriteria>()).Returns(missingMesssageInfo);

            //Act
            var actual = await service.GetMissingMessageDataAsync(missingMesssages);

            //Assert
            Assert.Equal(missingMesssageInfo, actual);
        }

        [Fact]
        public async Task GetMissingMessageInfoAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var missingMessageInfos = new List<MissingMesssageInfo> {
                new MissingMesssageInfo { JobIdentify = 1, VoidingHistoryCollectionId = 1, Content_en = "ABC", MessageId = 300, SettingName = "1" },
                new MissingMesssageInfo { JobIdentify = 2, VoidingHistoryCollectionId = 2000, Content_en = "ABC", MessageId = 2, SettingName = "2" },
            };

            var missedProblemAccounts = new List<MissingMesssageInfo>();
            var missedMonitoredPlayers = new List<MissingMesssageInfo>
            {
               new MissingMesssageInfo { JobIdentify = 5, VoidingHistoryCollectionId = 5, Content_en = "ABC", MessageId = 5, SettingName = "5" }
            };

            var msgList = new List<CurrentMaxMessageInfo>
            {
                new CurrentMaxMessageInfo {JobIdentify = 1, ReferenceValue = "1", SettingName = "1"},
                new CurrentMaxMessageInfo {JobIdentify = 2, ReferenceValue = "1000", SettingName = "2"},
                new CurrentMaxMessageInfo {JobIdentify = 4, ReferenceValue = "1000", SettingName = "4"},
                new CurrentMaxMessageInfo {JobIdentify = 5, ReferenceValue = "1000", SettingName = "5"}
            };

            var expected = new List<MissingMesssageInfo> {
                new MissingMesssageInfo { JobIdentify = 1, VoidingHistoryCollectionId = 1, Content_en = "ABC", MessageId = 300, SettingName = "1" },
                new MissingMesssageInfo { JobIdentify = 2, VoidingHistoryCollectionId = 2000, Content_en = "ABC", MessageId = 2, SettingName = "2"},
                new MissingMesssageInfo { JobIdentify = 5, VoidingHistoryCollectionId = 5, Content_en = "ABC", MessageId = 5, SettingName = "5" }
            };

            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingMessageDataCriteria>()).Returns(missingMessageInfos);
            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingProblemAccountCriteria>()).Returns(missedProblemAccounts);
            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingMonitoredPlayerCriteria>()).Returns(missedMonitoredPlayers);
            baseService.GetDataAsync<MissingMesssageInfo>(Arg.Any<GetMissingMessageNotificationCriteria>()).Returns(expected);
            var contentTypeList = fixture.Create<IEnumerable<MessageTypeInfo>>();
            var abnormalMissingMsgList = fixture.Create<IEnumerable<MessageRequestId>>();
            //Act
            var actual = await service.GetMissingMessageInfoAsync(msgList, contentTypeList, abnormalMissingMsgList);

            //Assert
            Assert.Equal("300", msgList.FirstOrDefault(x => x.JobIdentify == 1).MaxReferenceId);
            Assert.Equal("2000", msgList.FirstOrDefault(x => x.JobIdentify == 2).MaxReferenceId);
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
            var param = new List<MissingInfoIns> { new MissingInfoIns { MessageId = "1", JobIdentify = 1 } };
            var actual = await service.CreateMissingMessageAsync(param);

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
        }

        private ResetLastMessageIdService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            configuration = Substitute.For<IConfiguration>();
            commonService = Substitute.For<ICommonService>();
            return new ResetLastMessageIdService(baseService, configuration, commonService);
        }
    }
}