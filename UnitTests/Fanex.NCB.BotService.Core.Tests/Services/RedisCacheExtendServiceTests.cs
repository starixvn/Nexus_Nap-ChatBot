using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoFixture;
using AutoFixture.Kernel;
using Fanex.NCB.BotService.Core.Services;
using Fanex.NCB.BotService.Core.Models;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using StackExchange.Redis;
using Xunit;
using FluentAssertions;
using Fanex.Caching;


namespace Fanex.NCB.BotService.Core.Tests.Services
{

public class RedisCacheExtendServiceTests
{
    private readonly IFixture fixture = new Fixture();
    private IConnectionMultiplexer redisConnection;
    private ICacheService cacheService;
    private RedisConfiguration redisConfiguration;
    private RedisCacheExtendService redisCacheExtendService;

    public RedisCacheExtendServiceTests()
    {
        // Configure AutoFixture to handle EndPoint
        fixture.Customizations.Add(
            new TypeRelay(
                typeof(System.Net.EndPoint),
                typeof(System.Net.IPEndPoint)));

        redisConnection = Substitute.For<IConnectionMultiplexer>();
        cacheService = Substitute.For<ICacheService>();
        redisConfiguration = fixture.Create<RedisConfiguration>();
        redisCacheExtendService = new RedisCacheExtendService(redisConnection, cacheService, redisConfiguration);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange & Act
        var service = new RedisCacheExtendService(redisConnection, cacheService, redisConfiguration);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void GetValueByKeyContains_WithNoMatchingKeys_ReturnsEmptyEnumerable()
    {
        // Arrange
        var endPoint = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey>();

        redisConnection.GetEndPoints().Returns(new[] { endPoint });
        redisConnection.GetServer(endPoint).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>("testkey");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetValueByKeyContains_WithMatchingKeys_ReturnsValues()
    {
        // Arrange
        var endPoint = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey> { "testkey1", "testkey2" };
        var expectedValues = new[] { "value1", "value2" };

        redisConnection.GetEndPoints().Returns(new[] { endPoint });
        redisConnection.GetServer(endPoint).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        cacheService.Get<string>("testkey1").Returns("value1");
        cacheService.Get<string>("testkey2").Returns("value2");

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>("testkey");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("value1");
        result.Should().Contain("value2");
    }

    [Fact]
    public void GetValueByKeyContains_WithSomeNullValues_FiltersOutNulls()
    {
        // Arrange
        var endPoint = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey> { "testkey1", "testkey2" };

        redisConnection.GetEndPoints().Returns(new[] { endPoint });
        redisConnection.GetServer(endPoint).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        cacheService.Get<string>("testkey1").Returns("value1");
        cacheService.Get<string>("testkey2").Returns(default(string));

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>("testkey");

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain("value1");
    }

    [Fact]
    public void GetValueByKeyContains_WithComplexType_ReturnsTypedValues()
    {
        // Arrange
        var endPoint = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey> { "testkey1" };
        var expectedValue = fixture.Create<TestModel>();

        redisConnection.GetEndPoints().Returns(new[] { endPoint });
        redisConnection.GetServer(endPoint).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        cacheService.Get<TestModel>("testkey1").Returns(expectedValue);

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<TestModel>("testkey");

        // Assert
        result.Should().HaveCount(1);
        result.First().Should().Be(expectedValue);
    }

    [Fact]
    public void GetValueByKeyContains_WithMultipleEndPoints_UsesFirstEndPoint()
    {
        // Arrange
        var endPoint1 = fixture.Create<EndPoint>();
        var endPoint2 = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey> { "testkey1" };

        redisConnection.GetEndPoints().Returns(new[] { endPoint1, endPoint2 });
        redisConnection.GetServer(endPoint1).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        cacheService.Get<string>("testkey1").Returns("value1");

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>("testkey");

        // Assert
        redisConnection.Received(1).GetServer(endPoint1);
        redisConnection.DidNotReceive().GetServer(endPoint2);
    }

    [Fact]
    public void GetValueByKeyContains_WithNullEndPoints_ReturnsEmptyEnumerable()
    {
        // Arrange
        redisConnection.GetEndPoints().Returns(new EndPoint[0]);

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>("testkey");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetValueByKeyContains_WithEmptyKey_ReturnsEmptyEnumerable()
    {
        // Arrange
        var endPoint = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey>();

        redisConnection.GetEndPoints().Returns(new[] { endPoint });
        redisConnection.GetServer(endPoint).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>("");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetValueByKeyContains_WithNullKey_ReturnsEmptyEnumerable()
    {
        // Arrange
        var endPoint = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey>();

        redisConnection.GetEndPoints().Returns(new[] { endPoint });
        redisConnection.GetServer(endPoint).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetValueByKeyContains_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var endPoint = fixture.Create<EndPoint>();
        var server = Substitute.For<IServer>();
        var keys = new List<RedisKey> { "test-key_123", "test.key@domain" };

        redisConnection.GetEndPoints().Returns(new[] { endPoint });
        redisConnection.GetServer(endPoint).Returns(server);
        server.Keys(redisConfiguration.Db, Arg.Any<RedisValue>()).Returns(keys);

        cacheService.Get<string>("test-key_123").Returns("value1");
        cacheService.Get<string>("test.key@domain").Returns("value2");

        // Act
        var result = redisCacheExtendService.GetValueByKeyContains<string>("test");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("value1");
        result.Should().Contain("value2");
    }

    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
}
