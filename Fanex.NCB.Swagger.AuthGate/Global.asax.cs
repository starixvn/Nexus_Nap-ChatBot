using Autofac;
using Autofac.Integration.WebApi;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NCB.Swagger.AuthGate.Services;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Fanex.NCB.Swagger
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterDI();
        }
        void RegisterDI()
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            //register NAP API
            builder.Register(x => new UserService()).As<IUserService>();
            builder.Register(x => new MenuService()).As<IMenuService>();
            builder.RegisterType<AuthService>().AsImplementedInterfaces();


            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

          

        }
    }
}
