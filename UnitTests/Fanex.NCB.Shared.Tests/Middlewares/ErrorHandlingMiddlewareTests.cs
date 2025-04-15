namespace Fanex.NCB.Shared.Tests.Middlewares;

using System;
using System.Threading.Tasks;
using Fanex.NCB.Shared.Middlewares;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

public class ErrorHandlingMiddlewareTests
{
    private RequestDelegate next;
    private FakeLogger<ErrorHandlingMiddleware> logger;

    [Fact]
    public async Task WhenMiddleware_NotThrowException()
    {
        //Arrange
        var middleware = SetupMiddleware();
        var httpContext = Substitute.For<HttpContext>();

        //Action
        await middleware.InvokeAsync(httpContext);

        //Assert
        await next.Received().Invoke(Arg.Any<HttpContext>());
        Assert.Equal(0, logger.Collector.Count);
    }

    [Fact]
    public async Task WhenMiddleware_WhenThrowException_AndRedirect()
    {
        //Arrange
        var middleware = SetupMiddleware();
        var httpContext = Substitute.For<HttpContext>();
        next(Arg.Any<HttpContext>()).Throws(new Exception());

        //Act
        await middleware.InvokeAsync(httpContext);

        //Assert
        Assert.True(logger.Collector.Count > 0);
    }

    [Fact]
    public async Task WhenMiddleware_WhenThrowException_ReturnErrorCode()
    {
        var middleware = SetupMiddleware();
        var httpContext = Substitute.For<HttpContext>();
        httpContext.Request.Method = "post";
        httpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
        next(Arg.Any<HttpContext>()).Throws(new Exception());
        //Actual
        await middleware.InvokeAsync(httpContext);

        Assert.Equal(500, httpContext.Response.StatusCode);
    }

    private ErrorHandlingMiddleware SetupMiddleware()
    {
        next = Substitute.For<RequestDelegate>();
        logger = new FakeLogger<ErrorHandlingMiddleware>();
        return new ErrorHandlingMiddleware(next, logger);
    }
}