namespace Fanex.NCB.AutoJobs.Extensions
{
    using System.ServiceProcess;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Microsoft.AspNetCore.Hosting;

    public static class WebHostServiceExtension
    {
        public static void RunAsCustomService(this IWebHost host, bool isService)
        {
            var webHostService = new CustomWebHostService(host);
            if (isService)
            {
                ServiceBase.Run(webHostService);
            }
            else
            {
                webHostService.RunHangfireJobs();
                host.Run();
            }
        }
    }
}