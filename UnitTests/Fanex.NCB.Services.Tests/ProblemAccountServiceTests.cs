namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Criteria.ProblemAccount;
    using Fanex.NCB.Models.NotifictaionSendMessage;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Services.Tests.TestableClasses;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.ProblemAccounts;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class ProblemAccountServiceTests
    {
        private readonly IFixture fixture = new Fixture();
        private IBaseService baseService;
        private ICommonService commonService;
        private IChatbotService chatbotService;

        [Fact]
        public async Task SendMessageProblemAccountAsync_SettingIsNull_DoNothing()
        {
            // Arrange
            var criteria = fixture.Create<GetMessageSettingCriteria>();
            MessageSetting setting = null;
            var service = GetService();
            baseService.GetDataEntityAsync<MessageSetting>(Arg.Do<GetMessageSettingCriteria>(x => criteria = x)).Returns(setting);

            // Act
            await service.SendMessageProblemAccountAsync();

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<ProblemAccountInfo, short>(Arg.Any<GetProblemAccountInfoCriteria>());
        }

        [Fact]
        public async Task SendMessageProblemAccountAsync_NoDataToImport_DoNothing()
        {
            // Arrange
            var setting = fixture.Create<MessageSetting>();
            IEnumerable<ProblemAccountInfo> problemAccountInfo = new List<ProblemAccountInfo>();
            IEnumerable<short> categoryIds = new List<short>();
            IEnumerable<short> customerClass = new List<short>();
            var tuple = (problemAccountInfo, categoryIds, customerClass);

            var service = GetService();
            baseService.GetDataEntityAsync<MessageSetting>(Arg.Any<GetMessageSettingCriteria>()).Returns(setting);
            baseService.FetchMultipleAsync<ProblemAccountInfo, short, short>(Arg.Any<GetProblemAccountInfoCriteria>()).Returns(tuple);

            // Act
            await service.SendMessageProblemAccountAsync();

            // Assert
            await baseService.DidNotReceive().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportFromProblemAccountCriteria>());
        }

        [Fact]
        public async Task SendMessageProblemAccountAsync_HasDataToImport_DoAsExpected()
        {
            // Arrange
            var setting = fixture.Create<MessageSetting>();
            var problemAccountInfo = fixture.Create<IEnumerable<ProblemAccountInfo>>();
            var categoryIds = fixture.Create<IEnumerable<short>>();
            var customerClass = fixture.Create<IEnumerable<short>>();
            var tuple = (problemAccountInfo, categoryIds, customerClass);

            var service = GetService();
            baseService.GetDataEntityAsync<MessageSetting>(Arg.Any<GetMessageSettingCriteria>()).Returns(setting);
            baseService.FetchMultipleAsync<ProblemAccountInfo, short, short>(Arg.Any<GetProblemAccountInfoCriteria>()).Returns(tuple);

            // Act
            await service.SendMessageProblemAccountAsync();

            // Assert
            await baseService.Received().FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Any<ImportFromProblemAccountCriteria>());
        }

        [Fact]
        public async Task GetMessageSettingAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var criteria = fixture.Create<GetMessageSettingCriteria>();
            var setting = fixture.Create<MessageSetting>();

            baseService.GetDataEntityAsync<MessageSetting>(Arg.Do<GetMessageSettingCriteria>(x => criteria = x)).Returns(setting);

            //Act
            var actual = await service.GetMessageSettingAsync();

            //Assert
            Assert.Equal(setting, actual);
        }

        [Fact]
        public async Task GetProblemAccountAsync_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            var setting = fixture.Create<MessageSetting>();
            var criteria = fixture.Create<GetProblemAccountInfoCriteria>();
            var problemAccountInfo = fixture.Create<IEnumerable<ProblemAccountInfo>>();
            var categoryIds = fixture.Create<IEnumerable<short>>();
            var customerClass = fixture.Create<IEnumerable<short>>();
            var tuple = (problemAccountInfo, categoryIds, customerClass);

            baseService.FetchMultipleAsync<ProblemAccountInfo, short, short>(Arg.Do<GetProblemAccountInfoCriteria>(x => criteria = x)).Returns(tuple);

            //Act
            var actual = await service.GetProblemAccountAsync(setting);

            //Assert
            Assert.Equal(tuple, actual);
        }

        [Fact]
        public IEnumerable<CustomerClassInfo> GetAllCustomerClass_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            IEnumerable<short> customerClass = new List<short>() { 1 };
            IEnumerable<CustomerClassInfo> customerClassInfo = new List<CustomerClassInfo>
            {
                new CustomerClassInfo
                {
                    CustomerClass = "1",
                    CustomerClassNameEN = "test",
                    CustomerClassNameCS = "请"
                }
            };

            commonService.GetCustomerClassName("1", Language.en_US).Returns("test");
            commonService.GetCustomerClassName("1", Language.zh_CN).Returns("请");

            //Act
            var actual = service.GetAllCustomerClass(customerClass);

            //Assert
            //Assert.Equal(customerClassInfo, actual);

            return actual;
        }

        [Fact]
        public IEnumerable<Category> GetAllCategory_WhenCalled_ReturnExpectResult()
        {
            //Arrange
            var service = GetService();
            IEnumerable<short> categoriesId = new List<short>() { 1 };
            IEnumerable<Category> categories = new List<Category>
            {
                new Category
                {
                    CategoryId = "1",
                    CategoryNameEN = "test",
                    CategoryNameCS = "请"
                }
            };

            commonService.GetCategogyName("1", Language.en_US).Returns("test");
            commonService.GetCategogyName("1", Language.zh_CN).Returns("请");

            //Act
            var actual = service.GetAllCategory(categoriesId);

            //Assert
            //Assert.Equal(categories, actual);

            return actual;
        }

        [Fact]
        public async Task ImportMessageFromProblemAccountAsync_WhenCalled_ReturnAsExpectedResult()
        {
            // Arrange
            var messages = fixture.Create<IEnumerable<ProblemAccountInfo>>();
            var categoryids = fixture.Create<IEnumerable<short>>();
            var customerClass = fixture.Create<IEnumerable<short>>();

            var recipients = fixture.Create<IEnumerable<NotificationRecipient>>();
            var notificationMessages = fixture.Create<IEnumerable<NotificationSendMessage>>();
            var notifications = fixture.Create<IEnumerable<NotificationMessage>>();
            var tuple = (recipients, notificationMessages);
            var criteria = new ImportFromProblemAccountCriteria();

            var service = GetService();
            baseService.FetchMultipleAsync<NotificationRecipient, NotificationSendMessage>(Arg.Do<ImportFromProblemAccountCriteria>(x => criteria = x)).Returns(tuple);
            chatbotService.GenerateNotifificationMessageForCarbonGroupAsync(notificationMessages, recipients).Returns(notifications);

            // Act
            var result = await service.ImportMessageFromProblemAccountAsync(messages, categoryids, customerClass);

            // Assert
            Assert.Equal(JsonConvert.SerializeObject(messages), criteria.Message);
            Assert.Equal(JsonConvert.SerializeObject(service.GetAllCategory(categoryids)), criteria.CategoryInfo);
            Assert.Equal(JsonConvert.SerializeObject(service.GetAllCustomerClass(customerClass)), criteria.CustomerClassInfo);
            Assert.Equal(criteria.ErrCode, result.Item1.Code);
            Assert.Equal(criteria.ErrMsg, result.Item1.Message);

            Assert.Equal(notifications, result.Item2);
        }

        #region Helpers

        private ProblemAccountService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            commonService = Substitute.For<ICommonService>();
            chatbotService = Substitute.For<IChatbotService>();

            return Substitute.For<ProblemAccountService>(baseService, commonService, chatbotService);
        }

        #endregion Helpers
    }
}