namespace Fanex.NCB.AutoJobs
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using NLog.Web;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .UseNLog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls($"http://*:5000");
                    webBuilder.UseStartup<Startup>();
                })

                .Build()
                .Run();
        }
    }
}