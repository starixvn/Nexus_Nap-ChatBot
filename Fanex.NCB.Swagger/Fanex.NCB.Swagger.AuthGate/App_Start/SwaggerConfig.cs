using System.Web.Http;
using WebActivatorEx;
using Fanex.NCB.Swagger.AuthGate;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Fanex.NCB.Swagger.AuthGate
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "Fanex.NCB.Swagger.AuthGate");
                        c.PrettyPrint();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("My Swagger UI");
                    });
        }
    }
}
