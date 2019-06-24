using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fanex.NCB.Bot.Main
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 var env = hostingContext.HostingEnvironment;
                 config.SetBasePath(env.ContentRootPath);
                 config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                 config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                 config.AddEnvironmentVariables();
             })
              .ConfigureLogging((_, logging) =>
              {
                  logging.ClearProviders();
                  // add built-in providers manually, as needed
                  logging.AddConsole();
                  logging.AddDebug();
                  logging.AddEventLog();
                  logging.AddEventSourceLogger();
                  logging.AddTraceSource("TraceSourceSwitch");
              })
            .UseStartup<Startup>();
    }
}