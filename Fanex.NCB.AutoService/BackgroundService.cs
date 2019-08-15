namespace Fanex.NCB.AutoService
{
    using System.Configuration;
    using System.ServiceProcess;
    using Fanex.Common;
    using Hangfire;
    using Hangfire.MySql.Core;

    public partial class BackgroundService : ServiceBase
    {
        private BackgroundJobServer server;

        private readonly

        public BackgroundService()
        {
            InitializeComponent();
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            string decrypted = Cryptography.DeCryptConnectionString(connectionString);
            GlobalConfiguration.Configuration.UseStorage(new MySqlStorage(decrypted));
        }

        protected override void OnStart(string[] args)
        {
            server = new BackgroundJobServer();
        }

        protected override void OnStop()
        {
            server.Dispose();
        }
    }
}