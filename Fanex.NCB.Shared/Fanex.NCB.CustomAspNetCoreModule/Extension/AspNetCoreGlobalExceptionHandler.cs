using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace Fanex.NCB.CustomAspNetCoreModule
{
    public static class AspNetCoreGlobalExceptionHandler
    {
        private const int SERVER_ERROR = 500;
        public static IApplicationBuilder UseAspNetCoreExceptionHandler(this IApplicationBuilder app, string logCategory)
        {
            var loggerFactory = app.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;

            return app.UseExceptionHandler(HandleApiException(loggerFactory, logCategory));
        }

        public static Action<IApplicationBuilder> HandleApiException(ILoggerFactory loggerFactory, string logCategory)
        {
            return appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionHandlerFeature != null)
                    {
                        var logger = loggerFactory.CreateLogger($"{logCategory} GLOBAL ERROR");
                        logger.LogError(SERVER_ERROR, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                    }
                    var path = context.Response.HttpContext.Request.Path;

                    if (path.HasValue && path.Value.Contains("api"))
                    {
                        context.Response.StatusCode = SERVER_ERROR;
                        // await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                        await context.Response.WriteAsync(exceptionHandlerFeature.Error.Message + "\n" + exceptionHandlerFeature.Error.StackTrace + "\n" + exceptionHandlerFeature.Error.InnerException);

                        return;
                    }
                    // context.Response.Redirect($"/Error/Status/{context.Response.StatusCode}");
                    context.Response.WriteAsync(exceptionHandlerFeature.Error.StackTrace);
                });
            };
        }
    }
}
