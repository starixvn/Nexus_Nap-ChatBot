namespace Fanex.NCB.Main.Controllers.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NAP.Client;
using NAP.Client.Configuration;
using NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Core.Constants;
using Fanex.NCB.Core.Models;
using ViewModel.SettingConfiguration;
using Models.SettingConfiguration;
using Services.Interfaces;
using Fanex.NCB.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using static Fanex.NAP.Core.Business.Constant;

public class SettingConfigurationControllerTests
{
    private readonly ISettingConfigurationService settingConfigurationService;
    private readonly INapSession napSession;
    private readonly INapSettings napSettings;
    private readonly IUserService userService;
    private readonly SettingConfigurationController controller;
    private readonly IFixture fixture;

    public SettingConfigurationControllerTests()
    {
        settingConfigurationService = Substitute.For<ISettingConfigurationService>();
        napSession = Substitute.For<INapSession>();
        napSettings = Substitute.For<INapSettings>();
        userService = Substitute.For<IUserService>();
        NapClientManager.UseConfig(napSettings);
        controller = new SettingConfigurationController(napSession, settingConfigurationService, userService);
        fixture = new Fixture();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Index_WhenCalled_ReturnExpectedView(bool expectedPermission)
    {
        // Arrange
        var expectedName = "~/Views/SkypeBot/SettingConfiguration/Index.cshtml";
        userService.IsHavingPermission(napSession.UserId, napSettings.SiteId, FunctionKey.SettingManagement,
                Policy.Host)
            .Returns(expectedPermission);

        // Action
        var actual = controller.Index() as ViewResult;

        // Assert
        Assert.Equal(expectedPermission, Assert.IsType<bool>(actual.ViewData["HaveHostPermission"]));
        Assert.Equal(expectedName, actual.ViewName);
    }

    [Fact]
    public async Task GetSettings_ValidParams_ReturnsCorrectResult()
    {
        // Arrange
        var pageIndex = fixture.Create<int>();
        var pageSize = fixture.Create<int>();
        var settingType = fixture.Create<int?>();
        var total = fixture.Create<int>();
        var settingTypes = fixture.Create<IEnumerable<SettingType>>();
        var data = new ValueTuple<long, IEnumerable<SettingType>>(total, settingTypes);
        var expected = new { total = data.Item1, data = data.Item2 };
        settingConfigurationService.GetSettingsAsync(pageIndex, pageSize, settingType).Returns(data);

        // Action
        var actual = await controller.GetSettings(pageIndex, pageSize, settingType) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Create_ValidModel_ReturnsResult()
    {
        // Arrange
        var viewModel = fixture.Create<CreateSettingTypeViewModel>();
        var model = new CreateSettingType();
        var expected = fixture.Create<CommandResult>();
        settingConfigurationService.CreateAsync(Arg.Do<CreateSettingType>(x => model = x)).Returns(expected);

        // Action
        var actual = await controller.Create(viewModel) as JsonResult;

        // Assert
        Assert.Equal(model.Type, viewModel.Type);
        Assert.Equal(model.Name, viewModel.Name);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Create_InvalidModel_ReturnsErrors()
    {
        // Arrange
        var viewModel = new CreateSettingTypeViewModel();
        controller.ModelState.AddModelError("Name", "required");
        var message = controller.ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()
            .ErrorMessage;
        var expected = new { isSuccessful = false, message };

        // Action
        var actual = await controller.Create(viewModel) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetByType_ValidParams_ReturnsSettingType()
    {
        // Arrange
        var type = fixture.Create<int>();
        var expected = fixture.Create<SettingType>();
        settingConfigurationService.GetByTypeAsync(type).Returns(expected);

        // Action
        var actual = await controller.GetByType(type) as JsonResult;

        // Assert
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Update_ValidModel_ReturnsResult()
    {
        // Arrange
        var viewModel = fixture.Create<CreateSettingTypeViewModel>();
        var model = new UpdateSettingType();
        var expected = fixture.Create<CommandResult>();
        settingConfigurationService.UpdateAsync(Arg.Do<UpdateSettingType>(x => model = x)).Returns(expected);

        // Action
        var actual = await controller.Update(viewModel) as JsonResult;

        // Assert
        Assert.Equal(model.Type, viewModel.Type);
        Assert.Equal(model.Name, viewModel.Name);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Update_InvalidModel_ReturnsErrors()
    {
        // Arrange
        var viewModel = new CreateSettingTypeViewModel();
        controller.ModelState.AddModelError("Name", "required");
        var message = controller.ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()
            .ErrorMessage;
        var expected = new { isSuccessful = false, message };

        // Action
        var actual = await controller.Update(viewModel) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Delete_ValidParams_ReturnsResult()
    {
        // Arrange
        var type = fixture.Create<int>();
        var expected = fixture.Create<CommandResult>();
        settingConfigurationService.DeleteAsync(type, napSession.UserId).Returns(expected);

        // Actions
        var actual = await controller.Delete(type) as JsonResult;

        // Assert
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task Delete_InvalidModel_ReturnsErrors()
    {
        // Arrange
        var type = fixture.Create<int>();
        controller.ModelState.AddModelError("Test", "required");
        var message = controller.ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()
            .ErrorMessage;
        var expected = new { isSuccessful = false, message };

        // Action
        var actual = await controller.Delete(type) as JsonResult;

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
        userService.IsHavingPermission(napSession.UserId, napSettings.SiteId, FunctionKey.MappingSettingManagement,
                Policy.Host)
            .Returns(expectedPermission);

        // Action
        var actual = controller.Mapping() as ViewResult;

        // Assert
        Assert.Equal(expectedPermission, Assert.IsType<bool>(actual.ViewData["HaveHostPermission"]));
        Assert.Equal(expectedName, actual.ViewName);
    }

    [Fact]
    public async Task GetSettingTypes_WhenCalled_ReturnsAllSettingTypes()
    {
        // Arrange
        var expected = fixture.Create<IEnumerable<BaseModel>>();
        settingConfigurationService.GetAllSettingType().Returns(expected);

        // Action
        var actual = await controller.GetSettingTypes() as JsonResult;

        // Assert
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task GetJobInfo_ValidParams_ReturnsResult()
    {
        // Arrange
        var jobId = fixture.Create<int?>();
        var value = jobId ?? 1;
        var expected = fixture.Create<JobInfo>();
        settingConfigurationService.GetJobInfoAsync(value).Returns(expected);

        // Action
        var actual = await controller.GetJobInfo(jobId) as JsonResult;

        // Assert
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task GetMappings_ValidParams_ReturnsResult()
    {
        // Arrange
        var jobId = fixture.Create<int>();
        var pageIndex = fixture.Create<int>();
        var pageSize = fixture.Create<int>();
        var total = fixture.Create<int>();
        var mappings = fixture.Create<IEnumerable<Mapping>>();
        var data = new ValueTuple<long, IEnumerable<Mapping>>(total, mappings);
        var expected = new { total = data.Item1, data = data.Item2 };
        settingConfigurationService.GetMappingsAsync(jobId, pageIndex, pageSize).Returns(data);

        // Action
        var actual = await controller.GetMappings(jobId, pageIndex, pageSize) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateJobInfo_ValidModel_ReturnsResult()
    {
        // Arrange
        var viewModel = fixture.Create<UpdateJobInfoViewModel>();
        var model = new UpdateJobInfo();
        var expected = fixture.Create<CommandResult>();
        settingConfigurationService.UpdateJobInfoAsync(Arg.Do<UpdateJobInfo>(x => model = x), napSession.UserId)
            .Returns(expected);

        // Action
        var actual = await controller.UpdateJobInfo(viewModel) as JsonResult;

        // Assert
        Assert.Equal(model.BatchSize, viewModel.BatchSize);
        Assert.Equal(model.TestingGroup, viewModel.TestingGroup);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UpdateJobInfo_InvalidModel_ReturnsErrors()
    {
        // Arrange
        var viewModel = fixture.Create<UpdateJobInfoViewModel>();
        controller.ModelState.AddModelError("Test", "Test");
        var message = controller.ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()
            .ErrorMessage;
        var expected = new { isSuccessful = false, message };

        // Action
        var actual = await controller.UpdateJobInfo(viewModel) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetMsgContentsTypes_WhenCalled_GetAllMsgContentsTypes()
    {
        // Arrange
        var expected = fixture.Create<IEnumerable<SettingType>>();
        settingConfigurationService.GetMsgContentsTypesAsync().Returns(expected);

        // Action
        var actual = await controller.GetMsgContentsTypes() as JsonResult;

        // Assert
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task GetNotificationTypeData_WhenCalled_ReturnsNotificationTypeData()
    {
        // Arrange
        var type = fixture.Create<int>();
        var expected = fixture.Create<NotificationTypeData>();
        settingConfigurationService.GetNotificationTypeDataAsync(type).Returns(expected);

        // Action
        var actual = await controller.GetNotificationTypeData(type) as JsonResult;

        // Assert
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task CreateMapping_ValidModel_ReturnsResult()
    {
        // Arrange
        var viewModel = new CreateMappingViewModel();
        var model = new CreateMapping();
        var expected = fixture.Create<CommandResult>();
        settingConfigurationService.CreateMappingAsync(Arg.Do<CreateMapping>(x => model = x)).Returns(expected);

        // Action
        var actual = await controller.CreateMapping(viewModel) as JsonResult;

        // Assert
        Assert.Equal(viewModel.JobId, model.JobId);
        Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeID);
        Assert.Equal(viewModel.NotificationTemplateId, model.NotificationTemplateID);
        Assert.Equal(viewModel.MsgContentsType, model.MsgContentsType);
        Assert.Equal(viewModel.RequiredParam, model.RequiredParams);
        Assert.Equal(viewModel.IsTest, model.IsTest);
        Assert.Equal(napSession.UserId, model.CreatorId);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task CreateMapping_InvalidModel_ReturnsErrors()
    {
        // Arrange
        var viewModel = new CreateMappingViewModel();
        controller.ModelState.AddModelError("Test", "Test");
        var message = controller.ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()
            .ErrorMessage;
        var expected = new { isSuccessful = false, message };

        // Action
        var actual = await controller.CreateMapping(viewModel) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetMapping_WhenCalled_ReturnsMapping()
    {
        // Arrange
        var msgContentsType = fixture.Create<int>();
        var jobId = fixture.Create<int>();
        var model = new GetMapping();
        var expected = fixture.Create<Mapping>();
        settingConfigurationService.GetMappingAsync(Arg.Do<GetMapping>(x => model = x)).Returns(expected);

        // Action
        var actual = await controller.GetMapping(msgContentsType, jobId) as JsonResult;

        // Assert
        Assert.Equal(msgContentsType, model.MsgContentsType);
        Assert.Equal(jobId, model.JobId);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UpdateMapping_ValidModel_ReturnsResult()
    {
        // Arrange
        var viewModel = new UpdateMappingViewModel();
        var model = new UpdateMapping();
        var expected = fixture.Create<CommandResult>();
        settingConfigurationService.UpdateMappingAsync(Arg.Do<UpdateMapping>(x => model = x)).Returns(expected);

        // Action
        var actual = await controller.UpdateMapping(viewModel) as JsonResult;

        // Assert
        Assert.Equal(viewModel.JobId, model.JobId);
        Assert.Equal(viewModel.NotificationTypeId, model.NotificationTypeID);
        Assert.Equal(viewModel.NotificationTemplateId, model.NotificationTemplateID);
        Assert.Equal(viewModel.MsgContentsType, model.MsgContentsType);
        Assert.Equal(viewModel.RequiredParam, model.RequiredParams);
        Assert.Equal(viewModel.IsTest, model.IsTest);
        Assert.Equal(viewModel.OldMsgContentsType, model.OldMsgContentsType);
        Assert.Equal(napSession.UserId, model.ModifiedBy);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task UpdateMapping_InvalidModel_ReturnsErrors()
    {
        // Arrange
        var viewModel = new UpdateMappingViewModel();
        controller.ModelState.AddModelError("Test", "Test");
        var message = controller.ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()
            .ErrorMessage;
        var expected = new { isSuccessful = false, message };

        // Action
        var actual = await controller.UpdateMapping(viewModel) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task DeleteMapping_ValidModel_ReturnsResult()
    {
        // Arrange
        var templateId = fixture.Create<int>();
        var msgContentsType = fixture.Create<int>();
        var jobId = fixture.Create<int>();
        var model = new DeleteMapping();
        var expected = fixture.Create<CommandResult>();
        settingConfigurationService.DeleteMappingAsync(Arg.Do<DeleteMapping>(x => model = x)).Returns(expected);

        // Action
        var actual = await controller.DeleteMapping(templateId, msgContentsType, jobId) as JsonResult;

        // Assert
        Assert.Equal(jobId, model.JobId);
        Assert.Equal(templateId, model.TemlateId);
        Assert.Equal(msgContentsType, model.MsgContentsType);
        Assert.Equal(napSession.UserId, model.ModifiedBy);
        Assert.Equal(expected, actual.Value);
    }

    [Fact]
    public async Task DeleteMapping_InvalidModel_ReturnsErrors()
    {
        // Arrange
        var templateId = fixture.Create<int>();
        var msgContentsType = fixture.Create<int>();
        var jobId = fixture.Create<int>();
        controller.ModelState.AddModelError("Test", "Test");
        var message = controller.ModelState.FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors.FirstOrDefault()
            .ErrorMessage;
        var expected = new { isSuccessful = false, message };

        // Action
        var actual = await controller.DeleteMapping(templateId, msgContentsType, jobId) as JsonResult;

        // Assert
        actual.Value.Should().BeEquivalentTo(expected);
    }
}