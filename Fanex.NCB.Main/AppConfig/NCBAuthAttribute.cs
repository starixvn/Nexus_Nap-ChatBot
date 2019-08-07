namespace Fanex.NCB.Main.AppConfig
{
    using System;
    using Fanex.NAP.Client.Helpers.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NCBAuthAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly INapSession napSession;

        public NCBAuthAttribute() : this(null)
        {
        }

        public NCBAuthAttribute(INapSession napSession)
        {
            this.napSession = napSession;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (napSession.UserId > 0 || string.IsNullOrWhiteSpace(napSession.UserName))
            {
                context.HttpContext.Response.Redirect("/Auth/Index");
            }
        }
    }
}