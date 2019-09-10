namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.Setting;
    using Fanex.NCB.Models.Setting;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class SettingServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public void GetAllMappingStatus_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<BaseModel>
            {
                new BaseModel {Id = 0, Name = EnumHelper.GetDescription(MappingStatus.BotRemoved)},
                new BaseModel {Id = 1, Name = EnumHelper.GetDescription(MappingStatus.Mapped)},
                new BaseModel {Id = 2, Name = EnumHelper.GetDescription(MappingStatus.NotMapped)},
            };
            var service = GetService();

            //Act
            var actual = service.GetAllMappingStatus();

            //Assert
            Assert.Equal(expected.ToList().ToString(), actual.ToList().ToString());
        }

        [Fact]
        public async Task GetBotNameAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<BotName>
            {
                new BotName { Id = "1", Name = "1on1Notify"}
            };

            var criteria = new GetBotNameCriteria();
            var service = GetService();
            baseService.GetDataAsync<BotName>(Arg.Do<GetBotNameCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetBotNameAsync().ConfigureAwait(false);

            //Assert
            Assert.Equal("1", actual.FirstOrDefault().Id);
            Assert.Equal("1on1Notify", actual.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetChatGroupTypeAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<BaseModel>
            {
                new BaseModel { Id = 1, Name = "Test Recipient"}
            };

            var criteria = new GetFilterRecipientTypeCriteria();
            var service = GetService();
            baseService.GetDataAsync<BaseModel>(Arg.Do<GetFilterRecipientTypeCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetFilterRecipientTypesAsync().ConfigureAwait(false);

            //Assert
            Assert.Equal(1, actual.FirstOrDefault().Id);
            Assert.Equal("Test Recipient", actual.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetLicenseeStatusAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<BaseModel>
            {
                new BaseModel { Id = 1, Name = "Terminated"}
            };

            var criteria = new GetLicenseeStatusCriteria();
            var service = GetService();
            baseService.GetDataAsync<BaseModel>(Arg.Do<GetLicenseeStatusCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetLicenseeStatusAsync().ConfigureAwait(false);

            //Assert
            Assert.Equal(1, actual.FirstOrDefault().Id);
            Assert.Equal("Terminated", actual.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetFilterRecipientAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<ChatGroupType>
            {
                new ChatGroupType { Id = "1", Name = "Test Type"}
            };

            var criteria = new GetChatGroupTypeCriteria();
            var service = GetService();
            baseService.GetDataAsync<ChatGroupType>(Arg.Do<GetChatGroupTypeCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetChatGroupTypeAsync().ConfigureAwait(false);

            //Assert
            Assert.Equal("1", actual.FirstOrDefault().Id);
            Assert.Equal("Test Type", actual.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetRecipientsAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<Recipient>
            {
                new Recipient { Id = 1, Name = "10 Bet test"}
            };

            var criteria = new GetRecipientCriteria();
            var service = GetService();
            baseService.GetDataAsync<Recipient>(Arg.Do<GetRecipientCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetRecipientsAsync().ConfigureAwait(false);

            //Assert
            Assert.Equal(1, actual.FirstOrDefault().Id);
            Assert.Equal("10 Bet test", actual.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetRecipientTypeAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<BaseModel>
            {
                new BaseModel { Id = 1, Name = "Licensee"}
            };

            var criteria = new GetRecipientTypeCriteria();
            var service = GetService();
            baseService.GetDataAsync<BaseModel>(Arg.Do<GetRecipientTypeCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.GetRecipientTypesAsync().ConfigureAwait(false);

            //Assert
            Assert.Equal(1, actual.FirstOrDefault().Id);
            Assert.Equal("Licensee", actual.FirstOrDefault().Name);
        }

        [Fact]
        public async Task SearchChatGroupAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<BaseModel>
            {
                new BaseModel {Id = 1, Name = "Test"}
            };
            var criteria = new SearchChatGroupNameCriteria();
            var service = GetService();
            baseService.GetDataAsync<BaseModel>(Arg.Do<SearchChatGroupNameCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.SearchChatGroupAsync("Test").ConfigureAwait(false);

            //Assert
            Assert.Equal(1, actual.FirstOrDefault().Id);
            Assert.Equal("Test", actual.FirstOrDefault().Name);
        }

        [Fact]
        public async Task SearchRecipientAsync_WhenCalled_ReturnExpectedResult()
        {
            var expected = new List<Recipient>
            {
                new Recipient {Id = 1, Name = "Test"}
            };
            var criteria = new SearchRecipientCriteria();
            var service = GetService();
            baseService.GetDataAsync<Recipient>(Arg.Do<SearchRecipientCriteria>(x => criteria = x)).Returns(expected);

            //Act
            var actual = await service.SearchRecipientsAsync("Test").ConfigureAwait(false);

            //Assert
            Assert.Equal(1, actual.FirstOrDefault().Id);
            Assert.Equal("Test", actual.FirstOrDefault().Name);
        }

        private SettingService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            return new SettingService(baseService);
        }
    }
}