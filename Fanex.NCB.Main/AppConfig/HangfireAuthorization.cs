namespace Fanex.NCB.Core.Models
{
    using Fanex.NAP.Client.Helpers.Web;
    using Hangfire.Annotations;
    using Hangfire.Dashboard;

    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        private readonly INapSession napSession;

        public HangfireAuthorization() : this(null)
        {
        }

        public HangfireAuthorization(INapSession napSession)
        {
            this.napSession = napSession;
        }

        public bool Authorize([NotNull] DashboardContext context) => napSession.UserId > 0;
    }
}