using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fanex.NCB.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((builderContext, config) =>
                 {
                     var env = builderContext.HostingEnvironment;

                     config.SetBasePath(env.ContentRootPath);
                     config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                     config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                     config.AddJsonFile($"App_Data\\StoredProcedures\\SpName.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

                     config.AddEnvironmentVariables();
                 })
                 .ConfigureLogging((_, logging) => //should be move to startup.cs and add providers by ENV
                 {
                     logging.ClearProviders();
                     // add built-in providers manually, as needed
                     logging.AddFilter("NCB", LogLevel.Error);
                     logging.AddConsole();
                     logging.AddDebug();
                     logging.AddEventLog();
                     logging.AddEventSourceLogger();
                     logging.AddTraceSource("TraceSourceSwitch");
                 })
                .UseStartup<Startup>();
    }
}