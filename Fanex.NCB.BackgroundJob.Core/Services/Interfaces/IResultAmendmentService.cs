namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IResultAmendmentService
    {
        Task ResultAmendmentInternalCSDAsync();

        Task ResultAmendmentLicenseeAsync();
    }
}