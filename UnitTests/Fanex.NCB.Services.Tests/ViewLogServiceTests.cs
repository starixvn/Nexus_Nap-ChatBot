namespace Fanex.NCB.Services.Tests
{
    using AutoFixture;
    using Fanex.NCB.Criteria.ViewLog;
    using Fanex.NCB.Models.ViewLog;
    using Fanex.NCB.Shared.Services.Interfaces;
    using FluentAssertions;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class ViewLogServiceTests
    {
        private IBaseService baseService;
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task GetNotificationSystemMessage_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var total = fixture.Create<int>();
            var templates = fixture.Create<IEnumerable<ViewLogNotificationSystemMessage>>();
            var notificationId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();

            var criteria = new GetNotificationSystemMessageCriteria();

            baseService.FetchMultipleAsync<int, ViewLogNotificationSystemMessage>(Arg.Do<GetNotificationSystemMessageCriteria>(x => criteria = x))
                .Returns((new List<int> { total }, templates));

            var result = await service.GetNotificationSystemMessage(notificationId, pageIndex, pageSize).ConfigureAwait(false);

            Assert.Equal(notificationId, criteria.NotificationId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
        }

        [Fact]
        public async Task GetNotificationRecipientList_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var total = fixture.Create<int>();
            var templates = fixture.Create<IEnumerable<ViewLogNotificationRecipientList>>();
            var notificationId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();

            var criteria = new GetNotificationRecipientListCriteria();

            baseService.FetchMultipleAsync<int, ViewLogNotificationRecipientList>(Arg.Do<GetNotificationRecipientListCriteria>(x => criteria = x))
                .Returns((new List<int> { total }, templates));

            var result = await service.GetNotificationRecipientList(notificationId, pageIndex, pageSize).ConfigureAwait(false);

            Assert.Equal(notificationId, criteria.NotificationId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
        }

        [Fact]
        public async Task GetConversationMessageLog_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var total = fixture.Create<int>();
            var templates = fixture.Create<IEnumerable<ViewLogConversationMessageLog>>();
            var notificationId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();

            var criteria = new GetConversationMessageLogCriteria();

            baseService.FetchMultipleAsync<int, ViewLogConversationMessageLog>(Arg.Do<GetConversationMessageLogCriteria>(x => criteria = x))
                .Returns((new List<int> { total }, templates));

            var result = await service.GetConversationMessageLog(notificationId, pageIndex, pageSize).ConfigureAwait(false);

            Assert.Equal(notificationId, criteria.NotificationId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
        }

        [Fact]
        public async Task DeleteOldConversationMessageLogAsync_WhenCalled_ExecuteNonQueryAsync()
        {
            // Arrange
            var baseService = Substitute.For<IBaseService>();
            var service = new ViewLogService(baseService);
            var currentDate = _fixture.Create<string>();

            DeleteOldConversationMessageLogCommand command = null;
            await baseService.ExecuteNonQueryAsync(Arg.Do<DeleteOldConversationMessageLogCommand>(x => command = x));
                        
            // Act
            await service.DeleteOldConversationMessageLogAsync(currentDate);

            // Assert
            command.CurrentDate.Should().Be(currentDate);
        }

        private ViewLogService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return new ViewLogService(baseService);
        }
    }
}
