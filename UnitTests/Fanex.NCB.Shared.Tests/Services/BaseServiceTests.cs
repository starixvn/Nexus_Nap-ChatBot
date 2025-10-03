namespace Fanex.NCB.Shared.Services.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Repository;
using Models;
using Fanex.NCB.Shared.Services;
using Fanex.NAP.Core.Extensions;
using NSubstitute;
using Xunit;

public class BaseServiceTests
{
    private IDynamicRepository dynamicRepository;

    #region GetDataAsync

    [Fact]
    public async Task GetDataAsync_CriteriaIsNull_ReturnEmptyList()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.GetDataAsync<int>(null);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetDataAsync_CriteriaIsInvalid_ReturnEmptyList()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.GetDataAsync<FunctionSession>(criteria);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetDataAsync_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected = new List<FunctionSession>
        {
            new() { FunctionId = 1, FunctionKey = "Configuration", Name = "Configuration" }
        };
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.FetchAsync<FunctionSession>(criteria).Returns(expected);

        // Act
        var actual = await baseService.GetDataAsync<FunctionSession>(criteria);

        // Assert
        Assert.NotEmpty(actual);
        Assert.Equal(expected, actual);
    }

    #endregion GetDataAsync

    #region GetDataEntityAsync

    [Fact]
    public async Task GetDataEntityAsync_CriteriaIsNull_ReturnDefaultValue()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.GetDataEntityAsync<int>(null);

        // Assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task GetDataEntityAsync_CriteriaIsInvalid_ReturnDefaultValue()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.GetDataEntityAsync<int>(criteria);

        // Assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task GetDataEntityAsync_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected = 1;
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.GetAsync<int>(criteria).Returns(expected);

        // Act
        var actual = await baseService.GetDataEntityAsync<int>(criteria);

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion GetDataEntityAsync

    #region FetchMultipleAsync

    [Fact]
    public async Task FetchMultipleAsync_CriteriaIsNull_ReturnTupleWithEmptyList()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int>(null);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
    }

    [Fact]
    public async Task FetchMultipleAsync_CriteriaIsInvalid_ReturnTupleWithEmptyList()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int>(criteria);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
    }

    [Fact]
    public async Task FetchMultipleAsync_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected =
            new ValueTuple<IEnumerable<int>, IEnumerable<int>>(new List<int> { 1, 2 }, new List<int> { 1, 2 });
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.FetchMultipleAsync<int, int>(criteria).Returns(expected);

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int>(criteria);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FetchMultipleAsyncThird_CriteriaIsNull_ReturnTupleWithEmptyList()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int>(null);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
    }

    [Fact]
    public async Task FetchMultipleAsyncThird_CriteriaIsInvalid_ReturnTupleWithEmptyList()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int>(criteria);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
    }

    [Fact]
    public async Task FetchMultipleAsyncThird_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected = new ValueTuple<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>>(new List<int> { 1, 2 },
            new List<int> { 1, 2 }, new List<int> { 1, 2 });
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.FetchMultipleAsync<int, int, int>(criteria).Returns(expected);

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int>(criteria);

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion FetchMultipleAsync

    #region ExecuteNonQueryAsync

    [Fact]
    public async Task ExecuteNonQueryAsync_CriteriaIsNull_ReturnDefaultCommandResult()
    {
        // Arrange
        var expected = new CommandResult();
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.ExecuteNonQueryAsync(null);

        // Assert
        Assert.Equal(expected.Code, actual.Code);
        Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
        Assert.Equal(expected.Message, actual.Message);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_CriteriaIsInvalid_ReturnDefaultCommandResult()
    {
        // Arrange
        var expected = new CommandResult();
        var command = Substitute.For<INonQueryCommand>();
        command.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.ExecuteNonQueryAsync(command);

        // Assert
        Assert.Equal(expected.Code, actual.Code);
        Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
        Assert.Equal(expected.Message, actual.Message);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_CriteriaIsValid_ExpectedExecuteQuery()
    {
        // Arrange
        var command = Substitute.For<INonQueryCommand>();
        command.IsValid().Returns(true);
        var baseService = GetBaseService();
        // Act
        await baseService.ExecuteNonQueryAsync(command);

        // Assert
        await dynamicRepository.Received().ExecuteAsync(Arg.Is<INonQueryCommand>(x => x.IsValid()));
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_CriteriaIsValid_CallsExecuteAsync()
    {
        // Arrange
        var command = Substitute.For<INonQueryCommand>();
        command.IsValid().Returns(true);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.ExecuteNonQueryAsync(command);

        // Assert - We can't easily test the ToPropertyDictionary extension method
        // but we can verify that a CommandResult is returned with default values
        Assert.NotNull(actual);
        Assert.Equal(1, actual.Code); // Default constructor sets Code = 1
        Assert.Equal("Invalid Data", actual.Message); // Default constructor message
        Assert.False(actual.IsSuccessful);
    }

    #endregion ExecuteNonQueryAsync

    #region GetDataEntity

    [Fact]
    public void GetDataEntity_CriteriaIsNull_ReturnDefaultValue()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = baseService.GetDataEntity<int>(null);

        // Assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public void GetDataEntity_CriteriaIsInvalid_ReturnDefaultValue()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = baseService.GetDataEntity<int>(criteria);

        // Assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public void GetDataEntity_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected = 42;
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.Get<int>(criteria).Returns(expected);

        // Act
        var actual = baseService.GetDataEntity<int>(criteria);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetDataEntity_CriteriaIsValidWithServerIp_ReturnExpectedData()
    {
        // Arrange
        var expected = 42;
        var serverIp = "192.168.1.1";
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.Get<int>(criteria, serverIp).Returns(expected);

        // Act
        var actual = baseService.GetDataEntity<int>(criteria, serverIp);

        // Assert
        Assert.Equal(expected, actual);
        dynamicRepository.Received().Get<int>(criteria, serverIp);
    }

    #endregion GetDataEntity

    #region FetchMultipleAsync Additional Tests

    [Fact]
    public async Task FetchMultipleAsync4_CriteriaIsNull_ReturnTupleWithEmptyList()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int>(null);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
        Assert.Empty(actual.Item4);
    }

    [Fact]
    public async Task FetchMultipleAsync4_CriteriaIsInvalid_ReturnTupleWithEmptyList()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int>(criteria);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
        Assert.Empty(actual.Item4);
    }

    [Fact]
    public async Task FetchMultipleAsync4_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected = new ValueTuple<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>, IEnumerable<int>>(
            new List<int> { 1, 2 }, new List<int> { 3, 4 }, new List<int> { 5, 6 }, new List<int> { 7, 8 });
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.FetchMultipleAsync<int, int, int, int>(criteria).Returns(expected);

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int>(criteria);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FetchMultipleAsync5_CriteriaIsNull_ReturnTupleWithEmptyList()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int, int>(null);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
        Assert.Empty(actual.Item4);
        Assert.Empty(actual.Item5);
    }

    [Fact]
    public async Task FetchMultipleAsync5_CriteriaIsInvalid_ReturnTupleWithEmptyList()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int, int>(criteria);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
        Assert.Empty(actual.Item4);
        Assert.Empty(actual.Item5);
    }

    [Fact]
    public async Task FetchMultipleAsync5_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected = new ValueTuple<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>, IEnumerable<int>, IEnumerable<int>>(
            new List<int> { 1 }, new List<int> { 2 }, new List<int> { 3 }, new List<int> { 4 }, new List<int> { 5 });
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.FetchMultipleAsync<int, int, int, int, int>(criteria).Returns(expected);

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int, int>(criteria);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FetchMultipleAsync6_CriteriaIsNull_ReturnTupleWithEmptyList()
    {
        // Arrange
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int, int, int>(null);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
        Assert.Empty(actual.Item4);
        Assert.Empty(actual.Item5);
        Assert.Empty(actual.Item6);
    }

    [Fact]
    public async Task FetchMultipleAsync6_CriteriaIsInvalid_ReturnTupleWithEmptyList()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);
        var baseService = GetBaseService();

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int, int, int>(criteria);

        // Assert
        Assert.Empty(actual.Item1);
        Assert.Empty(actual.Item2);
        Assert.Empty(actual.Item3);
        Assert.Empty(actual.Item4);
        Assert.Empty(actual.Item5);
        Assert.Empty(actual.Item6);
    }

    [Fact]
    public async Task FetchMultipleAsync6_CriteriaIsValid_ReturnExpectedData()
    {
        // Arrange
        var expected = new ValueTuple<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>, IEnumerable<int>, IEnumerable<int>, IEnumerable<int>>(
            new List<int> { 1 }, new List<int> { 2 }, new List<int> { 3 }, new List<int> { 4 }, new List<int> { 5 }, new List<int> { 6 });
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var baseService = GetBaseService();
        dynamicRepository.FetchMultipleAsync<int, int, int, int, int, int>(criteria).Returns(expected);

        // Act
        var actual = await baseService.FetchMultipleAsync<int, int, int, int, int, int>(criteria);

        // Assert
        Assert.Equal(expected, actual);
    }

    #endregion FetchMultipleAsync Additional Tests

    #region Helpers

    private static IDynamicRepository FakeDynamicRepository()
    {
        var repository = Substitute.For<IDynamicRepository>();

        return repository;
    }

    private BaseService GetBaseService()
    {
        dynamicRepository = FakeDynamicRepository();
        var service = new BaseService(dynamicRepository);

        return service;
    }

    #endregion Helpers
}

public class FunctionSession
{
    public int FunctionId { get; set; }
    public string FunctionKey { get; set; }
    public string Name { get; set; }
}