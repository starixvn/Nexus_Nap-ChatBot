namespace Fanex.NCB.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Core.Enum;
    using Fanex.NCB.Core.Extensions;
    using Fanex.NCB.Core.Models;
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class SettingConfigurationServiceTests
    {
        private IBaseService baseService;
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task GetSettingAsync_WhenCalled_GetSettingsFromServices()
        {
            // Arrange
            var service = GetService();
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var settingType = _fixture.Create<int?>();
            var total = _fixture.Create<int>();
            var settingTypes = _fixture.Create<IEnumerable<SettingType>>();
            var criteria = new GetSettingTypeListCriteria();
            baseService.FetchMultipleAsync<long, SettingType>(Arg.Do<GetSettingTypeListCriteria>(x => criteria = x)).Returns((new List<long> { total }, settingTypes));

            // Act
            var result = await service.GetSettingsAsync(pageIndex, pageSize, settingType);

            // Assert
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(settingType, criteria.SettingType);
            Assert.Equal(total, result.Item1);
            Assert.Equal(settingTypes.Count(), result.Item2.Count());
        }

        [Fact]
        public async Task CreateAsync_ValidParams_ReturnsSuccessResult()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<CreateSettingType>();
            var command = new CreateSettingTypeCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateSettingTypeCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            // Action
            var result = await service.CreateAsync(model).ConfigureAwait(false);

            // Assert
            Assert.True(result.IsSuccessful);
            Assert.Equal(model.Type, command.Type);
            Assert.Equal(model.Name, command.Name);
            Assert.Equal(model.CreatorId, command.CreatorId);
        }

        [Fact]
        public async Task GetByTypeAsync_ValidParams_ReturnsSettingType()
        {
            // Arrange
            var service = GetService();
            var type = _fixture.Create<int>();
            var criteria = new GetByTypeCriteria();
            var setting = _fixture.Create<SettingType>();
            baseService.GetDataEntityAsync<SettingType>(Arg.Do<GetByTypeCriteria>(x => criteria = x)).Returns(setting);

            // Action
            var result = await service.GetByTypeAsync(type).ConfigureAwait(false);

            // Assert
            Assert.Equal(type, criteria.Type);
            Assert.Equal(setting, result);
        }

        [Fact]
        public async Task GetByTypeAsync_InvalidParams_ReturnsNull()
        {
            // Arrange
            var service = GetService();
            var type = 0;

            // Action
            var result = await service.GetByTypeAsync(type).ConfigureAwait(false);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ValidParams_ReturnsSuccessCommand()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<UpdateSettingType>();
            var command = new UpdateSettingTypeCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateSettingTypeCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            // Action
            var result = await service.UpdateAsync(model);

            // Assert
            Assert.Equal(model.Type, command.Type);
            Assert.Equal(model.Name, command.Name);
            Assert.Equal(model.ModifiedBy, command.ModifiedBy);
            Assert.Equal(0, result.Code);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task DeleteAsync_ValidParams_ReturnsSuccessCommand()
        {
            // Arrange
            var service = GetService();
            var type = _fixture.Create<int>();
            var modifiedBy = _fixture.Create<int>();
            var command = _fixture.Create<DeleteSettingTypeCommand>();
            baseService.ExecuteNonQueryAsync(Arg.Do<DeleteSettingTypeCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            // Action
            var result = await service.DeleteAsync(type, modifiedBy);

            // Assert
            Assert.Equal(type, command.Type);
            Assert.Equal(modifiedBy, command.ModifiedBy);
            Assert.Equal(0, result.Code);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public void GetAllSettingType_WhenCalled_ReturnsCorrectList()
        {
            // Arrange
            var service = GetService();
            var expected = Enum.GetValues(typeof(SettingTypes)).Cast<SettingTypes>().Select(v => new BaseModel
            {
                Id = (int)v,
                Name = EnumHelper.GetDescription(v)
            });

            // Action
            var result = service.GetAllSettingType();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetMappingAsync_ValidParams_ReturnsMappings()
        {
            // Arrange
            var service = GetService();
            var jobId = _fixture.Create<int>();
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var total = _fixture.Create<int>();
            var mappings = _fixture.Create<IEnumerable<Mapping>>();
            var criteria = new GetMappingsCriteria();
            baseService.FetchMultipleAsync<long, Mapping>(Arg.Do<GetMappingsCriteria>(x => criteria = x)).Returns((new List<long> { total }, mappings));

            // Action
            var result = await service.GetMappingsAsync(jobId, pageIndex, pageSize);

            // Assert
            Assert.Equal(jobId, criteria.JobId);
            Assert.Equal(pageIndex, criteria.PageIndex);
            Assert.Equal(pageSize, criteria.PageSize);
            Assert.Equal(total, result.Item1);
            Assert.Equal(mappings, result.Item2);
        }

        [Fact]
        public async Task UpdateJobInfoAsync_ValidParams_ReturnsSuccessCommand()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<UpdateJobInfo>();
            var command = new UpdateJobInfoCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateJobInfoCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            // Action
            var result = await service.UpdateJobInfoAsync(model);

            // Assert
            Assert.Equal(model.Id, command.Id);
            Assert.Equal(model.BatchSize, command.BatchSize);
            Assert.Equal(model.TestingGroup, command.TestingGroup);
            Assert.Equal(model.ModifiedBy, command.ModifiedBy);
            Assert.Equal(0, result.Code);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetMsgContentsTypesAsync_WhenCalled_ReturnsMsgContentsType()
        {
            // Arrange
            var service = GetService();
            var criteria = new GetMsgContentsTypesCriteria();
            var expectedResult = _fixture.Create<IEnumerable<SettingType>>();
            baseService.GetDataAsync<SettingType>(Arg.Do<GetMsgContentsTypesCriteria>(x => criteria = x)).Returns(expectedResult);

            // Action
            var result = await service.GetMsgContentsTypesAsync();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetNotificationTypeDataAsync_ValidParams_ReturnsCorrectData()
        {
            // Arrange
            var service = GetService();
            var notificationTypeId = _fixture.Create<int>();
            var expectedResult = _fixture.Create<NotificationTypeData>();
            var criteria = new GetNotificationTypeDataCriteria();
            baseService.GetDataEntityAsync<NotificationTypeData>(Arg.Do<GetNotificationTypeDataCriteria>(x => criteria = x)).Returns(expectedResult);

            // Action
            var result = await service.GetNotificationTypeDataAsync(notificationTypeId);

            // Assert
            Assert.Equal(notificationTypeId, criteria.NotificationTypeId);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task GetNotificationTypeDataAsync_InvalidParams_ReturnsNull()
        {
            // Arrange
            var service = GetService();
            var criteria = new GetNotificationTypeDataCriteria();

            // Action
            var result = await service.GetNotificationTypeDataAsync(0);

            // Assert
            Assert.Equal(0, criteria.NotificationTypeId);
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMappingAsync_ValidParams_ReturnsSuccessCommand()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<CreateMapping>();
            var command = new CreateMappingCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateMappingCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            // Action
            var result = await service.CreateMappingAsync(model);

            // Assert
            Assert.Equal(model.JobId, command.JobId);
            Assert.Equal(model.NotificationTypeID, command.NotificationTypeId);
            Assert.Equal(model.NotificationTemplateID, command.NotificationTemplateId);
            Assert.Equal(model.MsgContentsType, command.MsgContentsType);
            Assert.Equal(model.RequiredParams, command.RequiredParam);
            Assert.Equal(model.IsTest, command.IsTest);
            Assert.Equal(model.CreatorId, command.CreatorId);
            Assert.Equal(0, result.Code);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task GetMappingAsync_ValidParams_ReturnsCorrectData()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<GetMapping>();
            var expectedResult = _fixture.Create<Mapping>();
            var criteria = new GetMappingCriteria();
            baseService.GetDataEntityAsync<Mapping>(Arg.Do<GetMappingCriteria>(x => criteria = x)).Returns(expectedResult);

            // Action
            var result = await service.GetMappingAsync(model);

            // Assert
            Assert.Equal(model.TemlateId, criteria.NotificationTemplateId);
            Assert.Equal(model.MsgContentsType, criteria.MsgContentsType);
            Assert.Equal(model.JobId, criteria.JobId);
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task UpdateMappingAsync_ValidParams_ReturnsSuccessCommand()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<UpdateMapping>();
            var command = new UpdateMappingCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateMappingCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            // Action
            var result = await service.UpdateMappingAsync(model);

            // Assert
            Assert.Equal(model.NotificationTypeID, command.NotificationTypeId);
            Assert.Equal(model.JobId, command.JobId);
            Assert.Equal(model.NotificationTemplateID, command.NotificationTemplateId);
            Assert.Equal(model.MsgContentsType, command.MsgContentsType);
            Assert.Equal(model.RequiredParams, command.RequiredParam);
            Assert.Equal(model.IsTest, command.IsTest);
            Assert.Equal(model.ModifiedBy, command.ModifiedBy);
            Assert.Equal(model.OldTemplateId, command.OldTemplateId);
            Assert.Equal(model.OldMsgContentsType, command.OldMsgContentsType);
            Assert.Equal(0, result.Code);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task DeleteMappingAsync_ValidData_ReturnsSuccessCommand()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<DeleteMapping>();
            var command = new DeleteMappingCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<DeleteMappingCommand>(x => command = x)).Returns(new CommandResult { Code = 0 });

            // Action
            var result = await service.DeleteMappingAsync(model);

            // Assert
            Assert.Equal(model.JobId, command.JobId);
            Assert.Equal(model.TemlateId, command.NotificationTemplateId);
            Assert.Equal(model.MsgContentsType, command.MsgContentsType);
            Assert.Equal(model.ModifiedBy, command.ModifiedBy);
            Assert.Equal(0, result.Code);
            Assert.True(result.IsSuccessful);
        }

        private SettingConfigurationService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return new SettingConfigurationService(baseService);
        }
    }
}