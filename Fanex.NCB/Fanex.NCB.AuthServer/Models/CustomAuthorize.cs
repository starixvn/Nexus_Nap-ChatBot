using Fanex.NAP.Core.Configuration;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Fanex.NCB.AuthServer.Models
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        private static NapSettings _settings = ConfigurationManager.GetSection("NapSettings") as NapSettings;
        public override void OnAuthorization(HttpActionContext actionContext)
        {

            var authKey = actionContext.Request.Headers.GetValues("x-authKey")?.FirstOrDefault();
            if (string.IsNullOrEmpty(authKey) || !_settings.AccessKey.Equals(authKey))
            {
                HandleUnauthorizedRequest(actionContext);
            }

        }
    }
}