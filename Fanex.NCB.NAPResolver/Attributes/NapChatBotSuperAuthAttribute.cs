namespace Fanex.NCB.NAPResolver.Attributes
{
    using System;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Helpers.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NapChatBotSuperAuthAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly INapSession napSession;

        public NapChatBotSuperAuthAttribute() : this(null)
        {
        }

        internal NapChatBotSuperAuthAttribute(INapSession napSession)
        {
            this.napSession = napSession ?? NapClientManager.NapSession;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context?.HttpContext == null)
            {
                RequestHelper.CloseConnection(context);
                return;
            }

            if (napSession == null || napSession.UserId <= 0 || string.IsNullOrEmpty(napSession.UserName))
            {
                RequestHelper.CloseConnection(context);

                var path = context.HttpContext.Request.Path.HasValue ? context.HttpContext.Request.Path.Value : string.Empty;

                context.Result = new RedirectResult("~/Auth/Index?returnURL=" + path);
            }
        }
    }
}