using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Fanex.NCB.Criteria.NotificationType;
using Fanex.NCB.Models.NotificationType;
using Fanex.NCB.Shared.Services.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Fanex.NCB.Services.Tests
{
    public class NotificationTypeServiceTest
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task InsertAsync_WhenCalled_ExecuteNonQueryAsync()
        {
            // Arrange
            var baseService = Substitute.For<IBaseService>();
            var service = new NotificationTypeService(baseService);
            var model = _fixture.Create<NotificationTypeViewModel>();

            InsertNotificationTypeCommand command = _fixture.Create<InsertNotificationTypeCommand>();
            await baseService.ExecuteNonQueryAsync(Arg.Do<InsertNotificationTypeCommand>(x => command = x));

            // Act
            await service.InsertAsync(model);

            // Assert
            command.NotificationTypeName.Should().Be(model.NotificationTypeName);
            command.SystemId.Should().Be(model.SystemId);
            command.RecipientTypeIds.Should().Be(model.RecipientTypeIds);
            command.ChatGroupTypeIds.Should().Be(model.ChatGroupTypeIds);
            command.UserId.Should().Be(model.UserId);
        }

        [Fact]
        public async Task UpdateAsync_WhenCalled_ExecuteNonQueryAsync()
        {
            // Arrange
            var baseService = Substitute.For<IBaseService>();
            var service = new NotificationTypeService(baseService);
            var model = _fixture.Create<NotificationTypeViewModel>();

            UpdateNotificationTypeCommand command = _fixture.Create<UpdateNotificationTypeCommand>();
            await baseService.ExecuteNonQueryAsync(Arg.Do<UpdateNotificationTypeCommand>(x => command = x));

            // Act
            await service.UpdateAsync(model);

            // Assert
            command.NotificationTypeId.Should().Be(model.NotificationTypeId);
            command.NotificationTypeName.Should().Be(model.NotificationTypeName);
            command.SystemId.Should().Be(model.SystemId);
            command.RecipientTypeIds.Should().Be(model.RecipientTypeIds);
            command.ChatGroupTypeIds.Should().Be(model.ChatGroupTypeIds);
            command.UserId.Should().Be(model.UserId);
        }

        [Fact]
        public async Task DeleteAsync_WhenCalled_ExecuteNonQueryAsync()
        {
            // Arrange
            var baseService = Substitute.For<IBaseService>();
            var service = new NotificationTypeService(baseService);
            var notificationTypeId = _fixture.Create<int>();
            var userId = _fixture.Create<int>();

            DeleteNotificationTypeCommand command = null;
            await baseService.ExecuteNonQueryAsync(Arg.Do<DeleteNotificationTypeCommand>(x => command = x));

            // Act
            await service.DeleteAsync(notificationTypeId, userId);

            // Assert
            command.NotificationTypeId.Should().Be(notificationTypeId);
            command.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_GetNotificationTypesFromService()
        {
            // Arrange
            var baseService = Substitute.For<IBaseService>();
            var service = new NotificationTypeService(baseService);
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var data = FakeData();
            GetNotificationTypeCriteria criteria = null;

            baseService.FetchMultipleAsync<NotificationType, NotificationRecipientType, NotificationChatGroupType>(Arg.Do<GetNotificationTypeCriteria>(x => criteria = x)).Returns(data);

            // Act
            var result = await service.GetAllAsync(pageIndex, pageSize);

            // Assert
            criteria.PageIndex.Should().Be(pageIndex);
            criteria.PageSize.Should().Be(pageSize);
            result.Item1.Select(i => i.Id).Should().BeEquivalentTo(data.Item1.Select(i => i.Id));
            result.Item1.First().RecipientTypes.Should().BeEquivalentTo(data.Item2);
            result.Item1.Last().ChatGroupTypes.Should().BeEquivalentTo(data.Item3);
        }

        private ValueTuple<IEnumerable<NotificationType>,
            IEnumerable<NotificationRecipientType>,
            IEnumerable<NotificationChatGroupType>> FakeData()
        {
            var notificationTypes = _fixture.CreateMany<NotificationType>().ToList();
            var notificationRecipientTypes = _fixture.CreateMany<NotificationRecipientType>()
                .Select(x => { x.NotificationTypeId = notificationTypes.First().Id; return x; });
            var notificationChatGroupTypes = _fixture.CreateMany<NotificationChatGroupType>()
                .Select(x => { x.NotificationTypeId = notificationTypes.Last().Id; return x; });

            return new ValueTuple<IEnumerable<NotificationType>,
                IEnumerable<NotificationRecipientType>,
                IEnumerable<NotificationChatGroupType>>(notificationTypes, notificationRecipientTypes, notificationChatGroupTypes);
        }
    }
}