namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.ViewLog;
    using Fanex.NCB.Models.ViewLog;
    using Fanex.NCB.Shared.Services.Interfaces;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ViewLogServiceTests
    {
        private IBaseService baseService;
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task GetNotificationSystemMessageAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            var total = _fixture.Create<int>();
            var templates = _fixture.Create<IEnumerable<ViewLogNotificationSystemMessage>>();
            var notificationId = _fixture.Create<int?>();
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var criteria = new GetNotificationSystemMessageCriteria();

            // Act
            baseService.FetchMultipleAsync<int, ViewLogNotificationSystemMessage>(Arg.Do<GetNotificationSystemMessageCriteria>(x => criteria = x))
                .Returns((new List<int> { total }, templates));

            var result = await service.GetNotificationSystemMessageAsync(notificationId, pageIndex, pageSize).ConfigureAwait(false);

            // Assert
            Assert.Equal(notificationId, criteria.NotificationId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
        }

        [Fact]
        public async Task GetNotificationRecipientListAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            var total = _fixture.Create<int>();
            var templates = _fixture.Create<IEnumerable<ViewLogNotificationRecipientList>>();
            var notificationId = _fixture.Create<int?>();
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var criteria = new GetNotificationRecipientListCriteria();

            // Act
            baseService.FetchMultipleAsync<int, ViewLogNotificationRecipientList>(Arg.Do<GetNotificationRecipientListCriteria>(x => criteria = x))
                .Returns((new List<int> { total }, templates));

            var result = await service.GetNotificationRecipientListAsync(notificationId, pageIndex, pageSize).ConfigureAwait(false);

            // Assert
            Assert.Equal(notificationId, criteria.NotificationId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
        }

        [Fact]
        public async Task GetConversationMessageLogAsync_ValidData_ReturnExpectedResult()
        {
            // Arrange
            var service = GetService();

            var total = _fixture.Create<int>();
            var templates = _fixture.CreateMany<ViewLogConversationMessageLog>();
            var templateMap = _fixture.CreateMany<ViewLogConversationMessageLogRecipient>();
            var notificationId = _fixture.Create<int?>();
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var criteria = new GetConversationMessageLogCriteria();

            // Act
            baseService.FetchMultipleAsync<int, ViewLogConversationMessageLog, ViewLogConversationMessageLogRecipient>(Arg.Do<GetConversationMessageLogCriteria>(x => criteria = x))
                .Returns((new List<int> { total }, templates, templateMap));

            var result = await service.GetConversationMessageLogAsync(notificationId, pageIndex, pageSize);

            // Assert
            Assert.Equal(notificationId, criteria.NotificationId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
            Assert.Equal(templateMap.Count(), result.Item3.Count());
        }

        //[Fact]
        //public async Task DeleteOldConversationMessageLogAsync_WhenCalled_ExecuteNonQueryAsync()
        //{
        //    // Arrange
        //    var service = GetService();
        //    var currentDate = _fixture.Create<string>();

        //    DeleteOldConversationMessageLogCommand command = null;
        //    await baseService.ExecuteNonQueryAsync(Arg.Do<DeleteOldConversationMessageLogCommand>(x => command = x));

        //    // Act
        //    await service.DeleteOldConversationMessageLogAsync(currentDate);

        //    // Assert
        //    command.CurrentDate.Should().Be(currentDate);
        //}

        private ViewLogService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return new ViewLogService(baseService);
        }
    }
}