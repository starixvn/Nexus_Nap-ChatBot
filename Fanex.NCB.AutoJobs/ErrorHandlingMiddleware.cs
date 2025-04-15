namespace Fanex.NCB.AutoJobs;

using Fanex.Logging.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage(Justification = "Tested on other project")]
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            HandleException(context, ex);
        }
    }

    private void HandleException(HttpContext context, Exception exception)
    {
        var logMessage = new LogMessage(exception.Message)
            .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
            .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
            .ToString();
        logger.LogError(exception, logMessage);

        var method = context.Request.Method.ToLowerInvariant();
        var requestedWith = context.Request.Headers["X-Requested-With"];

        if ((method == "post" || method == "put" || method == "delete") && requestedWith == "XMLHttpRequest")
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}