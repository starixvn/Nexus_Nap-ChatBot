namespace Fanex.NCB.NAPResolver.Attributes;

using System;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

internal static class RequestHelper
{
    internal static void CloseConnection(AuthorizationFilterContext context)
    {
        if (IsAjaxRequest(context.HttpContext.Request))
        {
            var response = context.HttpContext.Response;
            const string messageContent = "<center><b>UnAuthorize</b></center>";
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            response.WriteAsync(messageContent);
        }
        else
        {
            context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    internal static bool IsAjaxRequest(HttpRequest request)
    {
        return string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal) ||
               string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.Ordinal);
    }
}