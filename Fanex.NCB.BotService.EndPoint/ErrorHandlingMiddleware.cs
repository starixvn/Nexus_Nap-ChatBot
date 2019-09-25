namespace Fanex.NCB.BotService.EndPoint
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Microsoft.AspNetCore.Http;

    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var x = context.Request.Host.Value;
                if (x.Contains("starific.net"))
                {
                    await logger.WarnAsync(x);
                }

                await next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                HandleException(context, ex);
            }
        }

        private void HandleException(HttpContext context, Exception exception)
        {
            string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}\n\rSource: {1}\n\rDomain: {2}\n\rTargetSite: {3}\n\rStackTrace: {4}",
                    exception.Message,
                    exception.Source,
                    context.Request.Headers["Host"],
                    exception.TargetSite,
                    exception.StackTrace);
            logger.Error(message);

            var method = context.Request.Method.ToLowerInvariant();
            var requestedWith = context.Request.Headers["X-Requested-With"];

            if ((method == "post" || method == "put" || method == "delete") && requestedWith == "XMLHttpRequest")
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            else
            {
                context.Response.Redirect("/error/InternalError");
            }
        }
    }
}