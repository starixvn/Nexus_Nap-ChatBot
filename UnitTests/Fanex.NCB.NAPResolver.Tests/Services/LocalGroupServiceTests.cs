namespace Fanex.NCB.NAPResolver.Tests;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Criteria.LocalNap.Group;
using LocalServices;
using Model;
using Shared.Services.Interfaces;
using NSubstitute;
using Xunit;

public class LocalGroupServiceTests
{
    private IBaseService baseService;

    [Fact]
    public async Task GetAsync_WhenCalled_ReturnExpectedResult()
    {
        var service = GetService();

        var fixture = new Fixture();
        var groupId = fixture.Create<int>();
        var expectedGroup = new FlatGroup
        {
            GroupId = 1,
            GroupName = "Test",
            IsActive = true,
            Description = "Test",
            CreatedDate = DateTime.UtcNow,
            CreatorId = 1,
            CreatorUsername = "Test",
            ModifierId = 1,
            ModifierUsername = "Test"
        };

        var criteria = new GetGroupByIdCriteria();

        baseService.GetDataEntityAsync<FlatGroup>(Arg.Any<GetGroupByIdCriteria>()).Returns(expectedGroup);

        var result = await service.GetAsync(groupId);

        Assert.Equal(expectedGroup.GroupId, result.Id);
        Assert.Equal(expectedGroup.GroupName, result.Name);
        Assert.Equal(expectedGroup.IsActive, result.IsActive);
    }

    [Fact]
    public async Task GetAsync_WithNullGroup_ReturnNull()
    {
        var service = GetService();
        var groupId = 999;

        baseService.GetDataEntityAsync<FlatGroup>(Arg.Any<GetGroupByIdCriteria>()).Returns((FlatGroup)null);

        var result = await service.GetAsync(groupId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_WithValidGroupId_SetsCriteriaCorrectly()
    {
        var service = GetService();
        var groupId = 123;
        var expectedGroup = new FlatGroup
        {
            GroupId = (short)groupId,
            GroupName = "Test Group",
            IsActive = true,
            Description = "Test Description",
            CreatedDate = DateTime.UtcNow,
            CreatorId = 1,
            CreatorUsername = "Creator",
            ModifierId = 1,
            ModifierUsername = "Modifier"
        };

        var capturedCriteria = new GetGroupByIdCriteria();
        baseService.GetDataEntityAsync<FlatGroup>(Arg.Do<GetGroupByIdCriteria>(x => capturedCriteria = x))
                   .Returns(expectedGroup);

        var result = await service.GetAsync(groupId);

        Assert.Equal(groupId, capturedCriteria.GroupId);
        Assert.NotNull(result);
        Assert.Equal(expectedGroup.GroupId, result.Id);
    }

    [Fact]
    public void Constructor_WithValidBaseService_CreatesInstance()
    {
        // Arrange
        var mockBaseService = Substitute.For<IBaseService>();

        // Act
        var service = new LocalGroupService(mockBaseService);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Get_ThrowsNotImplementedException()
    {
        // Arrange
        var service = GetService();
        var groupId = 1;

        // Act & Assert
        Assert.Throws<NotImplementedException>(() => service.Get(groupId));
    }

    #region Helpers

    private LocalGroupService GetService()
    {
        baseService = Substitute.For<IBaseService>();

        return Substitute.For<LocalGroupService>(baseService);
    }

    #endregion Helpers
}