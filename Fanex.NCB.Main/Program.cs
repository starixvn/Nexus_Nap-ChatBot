namespace Fanex.NCB.Main
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public static class Program
    {
        private const int MaxRequestLineSize = 32768;

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureKestrel((context, options) => { options.Limits.MaxRequestLineSize = MaxRequestLineSize; })
                .UseSentry();
        }
    }
}