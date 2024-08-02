namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISiteMessageService
    {
        Task PullMessageFromSiteMessageAsync();
    }
}