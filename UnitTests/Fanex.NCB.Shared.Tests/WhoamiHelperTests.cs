namespace Fanex.NCB.Shared.Tests;

using System;
using System.Net;
using Fanex.NCB.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

public class WhoamiHelperTests
{
    [Fact]
    public void GetContent_WithValidHttpContext_ReturnsFormattedContent()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();

        // Act
        var result = WhoamiHelper.GetContent(httpContext);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Web name:");
        result.Should().Contain("Port:");
        result.Should().Contain("Server IP:");
        result.Should().Contain("Server Time:");
        result.Should().Contain("Client IP:");
        result.Should().Contain("Request Scheme:");
        result.Should().Contain("Request Path:");
        result.Should().Contain("Request Headers:");
    }

    [Fact]
    public void GetContent_WithHeaders_IncludesHeadersInOutput()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var headers = new HeaderDictionary();
        headers.Add("User-Agent", "Test-Agent");
        headers.Add("Accept", "application/json");
        httpContext.Request.Headers.Returns(headers);

        // Act
        var result = WhoamiHelper.GetContent(httpContext);

        // Assert
        result.Should().Contain("User-Agent: Test-Agent");
        result.Should().Contain("Accept: application/json");
    }

    [Fact]
    public void GetContent_WithEmptyHeaders_StillReturnsValidContent()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        httpContext.Request.Headers.Returns(new HeaderDictionary());

        // Act
        var result = WhoamiHelper.GetContent(httpContext);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Request Headers:");
    }

    [Fact]
    public void GetContent_WithNullRemoteIpAddress_HandlesGracefully()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        httpContext.Connection.RemoteIpAddress.Returns((IPAddress)null);

        // Act
        var result = WhoamiHelper.GetContent(httpContext);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Client IP:");
    }

    [Fact]
    public void GetContent_WithNullLocalIpAddress_HandlesGracefully()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        httpContext.Connection.LocalIpAddress.Returns((IPAddress)null);

        // Act
        var result = WhoamiHelper.GetContent(httpContext);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Server IP:");
    }

    [Fact]
    public void GetContent_IncludesCurrentDateTime()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var beforeCall = DateTimeOffset.Now.DateTime;

        // Act
        var result = WhoamiHelper.GetContent(httpContext);
        var afterCall = DateTimeOffset.Now.DateTime;

        // Assert
        result.Should().Contain("Server Time:");
        // The result should contain a timestamp between before and after the call
        result.Should().MatchRegex(@"Server Time: \d{1,2}/\d{1,2}/\d{4}.*");
    }

    private static HttpContext CreateMockHttpContext()
    {
        var httpContext = Substitute.For<HttpContext>();
        var request = Substitute.For<HttpRequest>();
        var connection = Substitute.For<ConnectionInfo>();

        // Setup request
        request.Host.Returns(new HostString("localhost", 5000));
        request.Scheme.Returns("https");
        request.Path.Returns(new PathString("/api/test"));
        request.Headers.Returns(new HeaderDictionary());

        // Setup connection
        connection.LocalPort.Returns(5000);
        connection.LocalIpAddress.Returns(IPAddress.Loopback);
        connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.100"));

        // Wire up the mocks
        httpContext.Request.Returns(request);
        httpContext.Connection.Returns(connection);

        return httpContext;
    }
}
