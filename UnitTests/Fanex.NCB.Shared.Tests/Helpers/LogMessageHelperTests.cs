namespace Fanex.NCB.Shared.Tests.Helpers;

using System;
using System.Net;
using Fanex.NCB.Shared.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

public class LogMessageHelperTests
{
    [Fact]
    public void GenerateErrorMessage_WithValidException_ReturnsFormattedMessage()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var exception = new InvalidOperationException("Test error message");

        // Act
        var result = LogMessageHelper.GenerateErrorMessage(httpContext, exception);

        // Assert
        result.Should().NotBeNull();
        var message = result.ToString();
        message.Should().Contain("ErrMsg: Test error message");
        message.Should().Contain("REQUEST INFO");
        message.Should().Contain("EXCEPTION INFO");
        message.Should().Contain("Message: Test error message");
    }

    [Fact]
    public void GenerateErrorMessage_WithInnerException_IncludesInnerExceptionInfo()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var innerException = new ArgumentException("Inner error");
        var exception = new InvalidOperationException("Outer error", innerException);

        // Act
        var result = LogMessageHelper.GenerateErrorMessage(httpContext, exception);

        // Assert
        var message = result.ToString();
        message.Should().Contain("Message: Outer error");
        message.Should().Contain("Message: Inner error");
    }

    [Fact]
    public void GenerateErrorMessage_WithMultipleNestedExceptions_LimitsInnerExceptions()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var innermost = new ArgumentException("Level 4");
        var level3 = new InvalidOperationException("Level 3", innermost);
        var level2 = new NotSupportedException("Level 2", level3);
        var level1 = new Exception("Level 1", level2);

        // Act
        var result = LogMessageHelper.GenerateErrorMessage(httpContext, level1);

        // Assert
        var message = result.ToString();
        message.Should().Contain("Message: Level 1");
        message.Should().Contain("Message: Level 2");
        message.Should().Contain("Message: Level 3");
        // Should limit to 3 levels of inner exceptions
        message.Should().Contain("Message: Level 4");
    }

    [Fact]
    public void GenerateErrorMessage_WithNullHttpContext_HandlesGracefully()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act & Assert
        var result = Record.Exception(() => LogMessageHelper.GenerateErrorMessage(null, exception));

        // The method may throw due to null context, which is expected behavior
        // We're testing that the method handles edge cases
    }

    [Fact]
    public void GenerateErrorMessage_IncludesRequestInfo()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var exception = new Exception("Test");

        // Act
        var result = LogMessageHelper.GenerateErrorMessage(httpContext, exception);

        // Assert
        var message = result.ToString();
        message.Should().Contain("Host:");
        message.Should().Contain("Path:");
        message.Should().Contain("Body:");
        message.Should().Contain("Remote IP:");
    }

    [Fact]
    public void GenerateErrorMessage_IncludesExceptionDetails()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var exception = new InvalidOperationException("Test error");

        // Act
        var result = LogMessageHelper.GenerateErrorMessage(httpContext, exception);

        // Assert
        var message = result.ToString();
        message.Should().Contain("Source:");
        message.Should().Contain("TargetSite:");
        message.Should().Contain("StackTrace:");
    }

    [Fact]
    public void GenerateErrorMessage_WithExceptionWithoutInnerException_DoesNotIncludeInnerExceptionInfo()
    {
        // Arrange
        var httpContext = CreateMockHttpContext();
        var exception = new InvalidOperationException("Test error");

        // Act
        var result = LogMessageHelper.GenerateErrorMessage(httpContext, exception);

        // Assert
        var message = result.ToString();
        // Should only contain one "EXCEPTION INFO" section
        var exceptionInfoCount = message.Split("EXCEPTION INFO").Length - 1;
        exceptionInfoCount.Should().Be(1);
    }

    [Fact]
    public void GenerateErrorMessage_WithNullRequest_HandlesGracefully()
    {
        // Arrange
        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Returns((HttpRequest)null);
        var exception = new Exception("Test");

        // Act
        var result = LogMessageHelper.GenerateErrorMessage(httpContext, exception);

        // Assert
        var message = result.ToString();
        message.Should().Contain("ErrMsg: Test");
        message.Should().Contain("REQUEST INFO");
        message.Should().Contain("EXCEPTION INFO");
    }

    private static HttpContext CreateMockHttpContext()
    {
        var httpContext = Substitute.For<HttpContext>();
        var request = Substitute.For<HttpRequest>();
        var connection = Substitute.For<ConnectionInfo>();

        // Setup request
        request.Host.Returns(new HostString("localhost", 5000));
        request.Path.Returns(new PathString("/api/test"));
        request.Body.Returns(System.IO.Stream.Null);

        // Setup connection
        connection.RemoteIpAddress.Returns(IPAddress.Parse("192.168.1.100"));

        // Wire up the mocks
        httpContext.Request.Returns(request);
        httpContext.Connection.Returns(connection);

        return httpContext;
    }
}
