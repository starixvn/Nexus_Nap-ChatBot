namespace Fanex.NCB.Shared.Middlewares;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Fanex.NCB.Shared.Helpers;

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
            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception exception)
    {
        var logMessage = LogMessageHelper.GenerateErrorMessage(context, exception);
        logger.LogError(logMessage.ToString());

        var method = context.Request.Method.ToLowerInvariant();
        var requestedWith = context.Request.Headers["X-Requested-With"];

        if ((method == "post" || method == "put" || method == "delete") && requestedWith == "XMLHttpRequest")
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }

        return Task.CompletedTask;
    }
}