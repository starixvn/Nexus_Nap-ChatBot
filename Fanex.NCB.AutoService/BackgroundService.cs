namespace Fanex.NCB.AutoService
{
    using System.ServiceProcess;
    using Hangfire;

    public partial class BackgroundService : ServiceBase
    {
        private BackgroundJobServer _server;

        public BackgroundService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _server = new BackgroundJobServer();
        }

        protected override void OnStop()
        {
            _server.Dispose();
        }
    }
}