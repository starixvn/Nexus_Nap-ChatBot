namespace Fanex.NCB.AutoJobs.Extensions
{
    using System.ServiceProcess;
    using Fanex.NCB.AutoJobs.App_Configs;
    using Microsoft.AspNetCore.Hosting;

    public static class WebHostServiceExtension
    {
        public static void RunAsCustomService(this IWebHost host)
        {
            var webHostService = new CustomWebHostService(host);
            ServiceBase.Run(webHostService);
        }
    }
}