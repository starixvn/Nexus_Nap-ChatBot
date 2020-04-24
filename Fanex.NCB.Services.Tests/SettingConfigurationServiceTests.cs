namespace Fanex.NCB.Services.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.Criteria.SettingConfiguration;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
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
        public async Task CreateAsync_ValidData_ReturnsSuccessResult()
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
        public async Task CreateAsync_InvalidType_ReturnsUnsuccessResult()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<CreateSettingType>();
            model.Type = 0;
            var command = new CreateSettingTypeCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateSettingTypeCommand>(x => command = x)).Returns(new CommandResult());

            // Action
            var result = await service.CreateAsync(model).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(1, result.Code);
            Assert.Equal("Invalid Data", result.Message);
            Assert.Equal(model.Type, command.Type);
            Assert.Equal(model.Name, command.Name);
            Assert.Equal(model.CreatorId, command.CreatorId);
        }

        [Fact]
        public async Task CreateAsync_InvalidName_ReturnsUnsuccessResult()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<CreateSettingType>();
            model.Name = null;
            var command = new CreateSettingTypeCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateSettingTypeCommand>(x => command = x)).Returns(new CommandResult());

            // Action
            var result = await service.CreateAsync(model).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(1, result.Code);
            Assert.Equal("Invalid Data", result.Message);
            Assert.Equal(model.Type, command.Type);
            Assert.Equal(model.Name, command.Name);
            Assert.Equal(model.CreatorId, command.CreatorId);
        }

        [Fact]
        public async Task CreateAsync_InvalidCreatorId_ReturnsUnsuccessResult()
        {
            // Arrange
            var service = GetService();
            var model = _fixture.Create<CreateSettingType>();
            model.CreatorId = 0;
            var command = new CreateSettingTypeCommand();
            baseService.ExecuteNonQueryAsync(Arg.Do<CreateSettingTypeCommand>(x => command = x)).Returns(new CommandResult());

            // Action
            var result = await service.CreateAsync(model).ConfigureAwait(false);

            // Assert
            Assert.False(result.IsSuccessful);
            Assert.Equal(1, result.Code);
            Assert.Equal("Invalid Data", result.Message);
            Assert.Equal(model.Type, command.Type);
            Assert.Equal(model.Name, command.Name);
            Assert.Equal(model.CreatorId, command.CreatorId);
        }

        [Fact]
        public async Task GetByTypeAsync_ValidData_ReturnsSettingType()
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
        public async Task GetByTypeAsync_InvalidData_ReturnsNull()
        {
            // Arrange
            var service = GetService();
            var type = 0;

            // Action
            var result = await service.GetByTypeAsync(type).ConfigureAwait(false);

            // Assert
            Assert.Null(result);
        }

        private SettingConfigurationService GetService()
        {
            baseService = Substitute.For<IBaseService>();

            return new SettingConfigurationService(baseService);
        }
    }
}