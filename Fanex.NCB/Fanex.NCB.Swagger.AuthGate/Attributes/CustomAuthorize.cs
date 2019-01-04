using Fanex.NAP.Core.Configuration;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Fanex.NCB.Swagger.AuthGate.Attributes
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