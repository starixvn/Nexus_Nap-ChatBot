namespace Fanex.NCB.Shared.Helpers
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class LogMessageHelper
    {
        public static StringBuilder GenerateErrorMessage(HttpContext context, Exception exception)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"ErrMsg: {exception.Message}");

            AddRequestInfo(messageBuilder, context);

            const int limitedNumOfInnerException = 3;
            AddExceptionInfo(messageBuilder, exception, limitedNumOfInnerException);

            return messageBuilder;
        }

        private static void AddRequestInfo(StringBuilder builder, HttpContext context)
        {
            builder.AppendLine();
            builder.AppendLine("REQUEST INFO");

            var webInfo = GetRequestInfo(context);

            foreach (var item in webInfo)
            {
                builder.AppendLine($"{item.Key}: {item.Value}");
            }
        }

        private static void AddExceptionInfo(StringBuilder builder, Exception exception, int limitedInnerException)
        {
            builder.AppendLine();
            builder.AppendLine("EXCEPTION INFO");
            builder.AppendLine($"Message: {exception.Message}");
            builder.AppendLine($"Source: {exception.Source}");
            builder.AppendLine($"TargetSite: {exception.TargetSite}");
            builder.AppendLine($"StackTrace: {exception.StackTrace}");

            if (exception.InnerException != null && limitedInnerException > 0)
            {
                limitedInnerException--;
                AddExceptionInfo(builder, exception.InnerException, limitedInnerException);
            }
        }

        private static IDictionary<string, object> GetRequestInfo(HttpContext context)
        {
            var request = context.Request;
            return request == null
                ? new Dictionary<string, object>()
                : new Dictionary<string, object>
                {
                    [nameof(request.Host)] = request.Host,
                    [nameof(request.Path)] = request.Path,
                    [nameof(request.Body)] = request.Body,
                    ["Remote IP"] = context.Connection?.RemoteIpAddress
                };
        }
    }
}