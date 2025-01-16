namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Criteria.NotificationTemplate;
    using Models.NotificationTemplate;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class NotificationTemplaceServiceTests
    {
        private IBaseService baseService;

        [Fact]
        public async Task GetAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();

            var total = fixture.Create<long>();
            var templates = fixture.Create<IEnumerable<NotificationTemplate>>();
            var notificationTypeId = fixture.Create<int?>();
            var pageIndex = fixture.Create<int>();
            var pageSize = fixture.Create<int>();

            var criteria = new GetTemplatesCriteria();

            baseService.FetchMultipleAsync<long, NotificationTemplate>(Arg.Do<GetTemplatesCriteria>(x => criteria = x))
                .Returns((new List<long> { total }, templates));

            var result = await service.GetAsync(notificationTypeId, pageIndex, pageSize);

            Assert.Equal(notificationTypeId, criteria.NotificationTypeId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(templates.Count(), result.Item2.Count());
        }

        [Fact]
        public async Task CreateAsync_ValidData_ReturnSuccessResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var model = fixture.Create<CreateNotificationTemplate>();

            var command = new CreateNotificationTemplateCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<CreateNotificationTemplateCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            var result = await service.CreateAsync(model);

            Assert.True(result.IsSuccessful);
            Assert.Equal(model.NotificationTypeId, command.NotificationTypeId);
            Assert.Equal(model.NotificationTemplateName, command.NotificationTemplateName);
            Assert.Equal(model.Title, command.Title);
            Assert.Equal(model.Content, command.Content);
            Assert.Equal(model.CreatorId, command.CreatorId);
        }

        [Fact]
        public async Task EditAsync_ValidData_ReturnSuccessResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var model = fixture.Create<EditNotificationTemplate>();

            var command = new EditNotificationTemplateCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<EditNotificationTemplateCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            var result = await service.EditAsync(model);

            Assert.True(result.IsSuccessful);
            Assert.Equal(model.NotificationTypeId, command.NotificationTypeId);
            Assert.Equal(model.NotificationTemplateName, command.NotificationTemplateName);
            Assert.Equal(model.Title, command.Title);
            Assert.Equal(model.Content, command.Content);
            Assert.Equal(model.ModifierId, command.ModifierId);
        }

        [Fact]
        public async Task DeleteAsync_ValidData_ReturnSuccessResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationTemplateId = fixture.Create<int>();
            var userId = fixture.Create<int>();
            var command = new DeleteNotificationTemplateCommand();

            baseService.ExecuteNonQueryAsync(Arg.Do<DeleteNotificationTemplateCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            var result = await service.DeleteAsync(notificationTemplateId, userId);

            Assert.True(result.IsSuccessful);
            Assert.Equal(notificationTemplateId, command.NotificationTemplateId);
            Assert.Equal(userId, command.UserId);
        }

        [Fact]
        public async Task GetByIdAsync_ValidData_ReturnExpectedResult()
        {
            var service = GetService();

            var fixture = new Fixture();
            var notificationTemplateId = fixture.Create<int>();
            var template = fixture.Create<NotificationTemplate>();

            var criteria = new GetTemplateByIdCriteria();

            baseService.GetDataEntityAsync<NotificationTemplate>(Arg.Do<GetTemplateByIdCriteria>(x => criteria = x))
                .Returns(template);

            var result = await service.GetByIdAsync(notificationTemplateId);

            Assert.Equal(notificationTemplateId, criteria.NotificationTemplateId);
            Assert.Equal(template.NotificationTemplateName, result.NotificationTemplateName);
            Assert.Equal(template.Title, result.Title);
            Assert.Equal(template.Content, result.Content);
            Assert.Equal(template.NotificationTypeId, result.NotificationTypeId);
            Assert.Equal(template.NotificationTypeName, result.NotificationTypeName);
        }

        private NotificationTemplaceService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return new NotificationTemplaceService(baseService);
        }
    }
}