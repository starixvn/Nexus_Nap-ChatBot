namespace Fanex.NCB.Main.Controllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NCB.Core.Constants;
    using Fanex.NCB.Main.ViewModel.SettingConfiguration;
    using Fanex.NCB.Models.SettingConfiguration;
    using Fanex.NCB.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;
    using static Fanex.NAP.Core.Business.Constant;
    using Fanex.NCB.Core.Models;

    public class SettingConfigurationControllerTests
    {
        private readonly ISettingConfigurationService _settingConfigurationService;
        private readonly INapSession _napSession;
        private readonly INapSettings _napSettings;
        private readonly IUserService _userService;
        private readonly SettingConfigurationController _controller;
        private readonly IFixture _fixture;

        public SettingConfigurationControllerTests()
        {
            _settingConfigurationService = Substitute.For<ISettingConfigurationService>();
            _napSession = Substitute.For<INapSession>();
            _napSettings = Substitute.For<INapSettings>();
            _userService = Substitute.For<IUserService>();
            NapClientManager.UseConfig(_napSettings);
            _controller = new SettingConfigurationController(_napSession, _settingConfigurationService);
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Index_WhenCalled_ReturnExpectedView(bool expectedPermission)
        {
            // Arrange
            var expectedName = "~/Views/SkypeBot/SettingConfiguration/Index.cshtml";
            _userService.IsHavingPermission(_napSession.UserId, _napSettings.SiteId, FunctionKey.SettingConfiguration, Policy.Host)
                .Returns(expectedPermission);

            // Action
            var actual = _controller.Index(_userService) as ViewResult;

            // Assert
            Assert.Equal(expectedPermission, Assert.IsType<bool>(actual.ViewData["HaveHostPermission"]));
            Assert.Equal(expectedName, actual.ViewName);
        }

        [Fact]
        public async Task GetSettings_ValidParams_ReturnsCorrectResult()
        {
            // Arrange
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var settingType = _fixture.Create<int?>();
            var total = _fixture.Create<int>();
            var settingTypes = _fixture.Create<IEnumerable<SettingType>>();
            var data = new ValueTuple<long, IEnumerable<SettingType>>(total, settingTypes);
            var expected = new { total = data.Item1, data = data.Item2 };
            _settingConfigurationService.GetSettingsAsync(pageIndex, pageSize, settingType).Returns(data);

            // Action
            var actual = await _controller.GetSettings(pageIndex, pageSize, settingType) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Create_ValidModel_ReturnsResult()
        {
            // Arrange
            var viewModel = _fixture.Create<CreateSettingTypeViewModel>();
            var model = new CreateSettingType();
            var expected = _fixture.Create<CommandResult>();
            _settingConfigurationService.CreateAsync(Arg.Do<CreateSettingType>(x => model = x)).Returns(expected);

            // Action
            var actual = await _controller.Create(viewModel) as JsonResult;

            // Assert
            Assert.Equal(model.Type, viewModel.Type);
            Assert.Equal(model.Name, viewModel.Name);
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsErrors()
        {
            // Arrange
            var viewModel = _fixture.Create<CreateSettingTypeViewModel>();
            _controller.ModelState.AddModelError("Name", "required");
            var errors = _controller.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            var expected = new { isSuccessful = false, errors };

            // Action
            var actual = await _controller.Create(viewModel) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetByType_ValidParams_ReturnsSettingType()
        {
            // Arrange
            var type = _fixture.Create<int>();
            var expected = _fixture.Create<SettingType>();
            _settingConfigurationService.GetByTypeAsync(type).Returns(expected);

            // Action
            var actual = await _controller.GetByType(type) as JsonResult;

            // Assert
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task Update_ValidModel_ReturnsResult()
        {
            // Arrange
            var viewModel = _fixture.Create<CreateSettingTypeViewModel>();
            var model = new UpdateSettingType();
            var expected = _fixture.Create<CommandResult>();
            _settingConfigurationService.UpdateAsync(Arg.Do<UpdateSettingType>(x => model = x)).Returns(expected);

            // Action
            var actual = await _controller.Update(viewModel) as JsonResult;

            // Assert
            Assert.Equal(model.Type, viewModel.Type);
            Assert.Equal(model.Name, viewModel.Name);
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task Update_InvalidModel_ReturnsErrors()
        {
            // Arrange
            var viewModel = _fixture.Create<CreateSettingTypeViewModel>();
            _controller.ModelState.AddModelError("Name", "required");
            var errors = _controller.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            var expected = new { isSuccessful = false, errors };

            // Action
            var actual = await _controller.Update(viewModel) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Delete_ValidParams_ReturnsResult()
        {
            // Arrange
            var type = _fixture.Create<int>();
            var expected = _fixture.Create<CommandResult>();
            _settingConfigurationService.DeleteAsync(type, _napSession.UserId).Returns(expected);

            // Actions
            var actual = await _controller.Delete(type) as JsonResult;

            // Assert
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task Delete_InvalidModel_ReturnsErrors()
        {
            // Arrange
            var type = _fixture.Create<int>();
            _controller.ModelState.AddModelError("Test", "required");
            var errors = _controller.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            var expected = new { isSuccessful = false, errors };

            // Action
            var actual = await _controller.Delete(type) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Mapping_WhenCalled_ReturnExpectedView(bool expectedPermission)
        {
            // Arrange
            var expectedName = "~/Views/SkypeBot/SettingConfiguration/Mapping.cshtml";
            _userService.IsHavingPermission(_napSession.UserId, _napSettings.SiteId, FunctionKey.SettingConfiguration, Policy.Host)
                .Returns(expectedPermission);

            // Action
            var actual = _controller.Mapping(_userService) as ViewResult;

            // Assert
            Assert.Equal(expectedPermission, Assert.IsType<bool>(actual.ViewData["HaveHostPermission"]));
            Assert.Equal(expectedName, actual.ViewName);
        }

        [Fact]
        public void GetSettingTypes_WhenCalled_ReturnsAllSettingTypes()
        {
            // Arrange
            var expected = _fixture.Create<IEnumerable<BaseModel>>();
            _settingConfigurationService.GetAllSettingType().Returns(expected);

            // Action
            var actual = _controller.GetSettingTypes() as JsonResult;

            // Assert
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task GetJobInfo_ValidParams_ReturnsResult()
        {
            // Arrange
            var jobId = _fixture.Create<int?>();
            var value = jobId ?? 1;
            var expected = _fixture.Create<JobInfo>();
            _settingConfigurationService.GetJobInfoAsync(value).Returns(expected);

            // Action
            var actual = await _controller.GetJobInfo(jobId) as JsonResult;

            // Assert
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task GetMappings_ValidParams_ReturnsResult()
        {
            // Arrange
            var jobId = _fixture.Create<int>();
            var pageIndex = _fixture.Create<int>();
            var pageSize = _fixture.Create<int>();
            var total = _fixture.Create<int>();
            var mappings = _fixture.Create<IEnumerable<Mapping>>();
            var data = new ValueTuple<long, IEnumerable<Mapping>>(total, mappings);
            var expected = new { total = data.Item1, data = data.Item2 };
            _settingConfigurationService.GetMappingsAsync(jobId, pageIndex, pageSize).Returns(data);

            // Action
            var actual = await _controller.GetMappings(jobId, pageIndex, pageSize) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task UpdateJobInfo_ValidModel_ReturnsResult()
        {
            // Arrange
            var viewModel = _fixture.Create<UpdateJobInfoViewModel>();
            var model = new UpdateJobInfo();
            var expected = _fixture.Create<CommandResult>();
            _settingConfigurationService.UpdateJobInfoAsync(Arg.Do<UpdateJobInfo>(x => model = x)).Returns(expected);

            // Action
            var actual = await _controller.UpdateJobInfo(viewModel) as JsonResult;

            // Assert
            Assert.Equal(model.Id, viewModel.Id);
            Assert.Equal(model.BatchSize, viewModel.BatchSize);
            Assert.Equal(model.TestingGroup, viewModel.TestingGroup);
            Assert.Equal(model.ModifiedBy, _napSession.UserId);
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task UpdateJobInfo_InvalidModel_ReturnsErrors()
        {
            // Arrange
            var viewModel = _fixture.Create<UpdateJobInfoViewModel>();
            _controller.ModelState.AddModelError("Test", "Test");
            var errors = _controller.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            var expected = new { isSuccessful = false, errors };

            // Action
            var actual = await _controller.UpdateJobInfo(viewModel) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetMsgContentsTypes_WhenCalled_GetAllMsgContentsTypes()
        {
            // Arrange
            var expected = _fixture.Create<IEnumerable<SettingType>>();
            _settingConfigurationService.GetMsgContentsTypesAsync().Returns(expected);

            // Action
            var actual = await _controller.GetMsgContentsTypes() as JsonResult;

            // Assert
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task GetNotificationTypeData_WhenCalled_ReturnsNotificationTypeData()
        {
            // Arrange
            var type = _fixture.Create<int>();
            var expected = _fixture.Create<NotificationTypeData>();
            _settingConfigurationService.GetNotificationTypeDataAsync(type).Returns(expected);

            // Action
            var actual = await _controller.GetNotificationTypeData(type) as JsonResult;

            // Assert
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task CreateMapping_ValidModel_ReturnsResult()
        {
            // Arrange
            var viewModel = _fixture.Create<CreateMappingViewModel>();
            var model = new CreateMapping();
            var expected = _fixture.Create<CommandResult>();
            _settingConfigurationService.CreateMappingAsync(Arg.Do<CreateMapping>(x => model = x)).Returns(expected);

            // Action
            var actual = await _controller.CreateMapping(viewModel) as JsonResult;

            // Assert
            Assert.Equal(viewModel.JobId, model.JobId);
            Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeID);
            Assert.Equal(viewModel.NotificationTemplateId, model.NotificationTemplateID);
            Assert.Equal(viewModel.MsgContentsType, model.MsgContentsType);
            Assert.Equal(viewModel.RequiredParam, model.RequiredParams);
            Assert.Equal(viewModel.IsTest, model.IsTest);
            Assert.Equal(_napSession.UserId, model.CreatorId);
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task CreateMapping_InvalidModel_ReturnsErrors()
        {
            // Arrange
            var viewModel = _fixture.Create<CreateMappingViewModel>();
            _controller.ModelState.AddModelError("Test", "Test");
            var errors = _controller.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            var expected = new { isSuccessful = false, errors };

            // Action
            var actual = await _controller.CreateMapping(viewModel) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetMapping_WhenCalled_ReturnsMapping()
        {
            // Arrange
            var templateId = _fixture.Create<int>();
            var msgContentsType = _fixture.Create<int>();
            var jobId = _fixture.Create<int>();
            var model = new GetMapping();
            var expected = _fixture.Create<Mapping>();
            _settingConfigurationService.GetMappingAsync(Arg.Do<GetMapping>(x => model = x)).Returns(expected);

            // Action
            var actual = await _controller.GetMapping(templateId, msgContentsType, jobId) as JsonResult;

            // Assert
            Assert.Equal(templateId, model.TemlateId);
            Assert.Equal(msgContentsType, model.MsgContentsType);
            Assert.Equal(jobId, model.JobId);
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task UpdateMapping_ValidModel_ReturnsResult()
        {
            // Arrange
            var viewModel = _fixture.Create<UpdateMappingViewModel>();
            var model = new UpdateMapping();
            var expected = _fixture.Create<CommandResult>();
            _settingConfigurationService.UpdateMappingAsync(Arg.Do<UpdateMapping>(x => model = x)).Returns(expected);

            // Action
            var actual = await _controller.UpdateMapping(viewModel) as JsonResult;

            // Assert
            Assert.Equal(viewModel.JobId, model.JobId);
            Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeID);
            Assert.Equal(viewModel.NotificationTemplateId, model.NotificationTemplateID);
            Assert.Equal(viewModel.MsgContentsType, model.MsgContentsType);
            Assert.Equal(viewModel.RequiredParam, model.RequiredParams);
            Assert.Equal(viewModel.IsTest, model.IsTest);
            Assert.Equal(viewModel.OldTemplateId, model.OldTemplateId);
            Assert.Equal(viewModel.OldMsgContentsType, model.OldMsgContentsType);
            Assert.Equal(_napSession.UserId, model.ModifiedBy);
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task UpdateMapping_InvalidModel_ReturnsErrors()
        {
            // Arrange
            var viewModel = _fixture.Create<UpdateMappingViewModel>();
            _controller.ModelState.AddModelError("Test", "Test");
            var errors = _controller.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            var expected = new { isSuccessful = false, errors };

            // Action
            var actual = await _controller.UpdateMapping(viewModel) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task DeleteMapping_ValidModel_ReturnsResult()
        {
            // Arrange
            var templateId = _fixture.Create<int>();
            var msgContentsType = _fixture.Create<int>();
            var jobId = _fixture.Create<int>();
            var model = new DeleteMapping();
            var expected = _fixture.Create<CommandResult>();
            _settingConfigurationService.DeleteMappingAsync(Arg.Do<DeleteMapping>(x => model = x)).Returns(expected);

            // Action
            var actual = await _controller.DeleteMapping(templateId, msgContentsType, jobId) as JsonResult;

            // Assert
            Assert.Equal(jobId, model.JobId);
            Assert.Equal(templateId, model.TemlateId);
            Assert.Equal(msgContentsType, model.MsgContentsType);
            Assert.Equal(_napSession.UserId, model.ModifiedBy);
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public async Task DeleteMapping_InvalidModel_ReturnsErrors()
        {
            // Arrange
            var templateId = _fixture.Create<int>();
            var msgContentsType = _fixture.Create<int>();
            var jobId = _fixture.Create<int>();
            _controller.ModelState.AddModelError("Test", "Test");
            var errors = _controller.ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors.First().ErrorMessage });
            var expected = new { isSuccessful = false, errors };

            // Action
            var actual = await _controller.DeleteMapping(templateId, msgContentsType, jobId) as JsonResult;

            // Assert
            actual.Value.Should().BeEquivalentTo(expected);
        }
    }
}