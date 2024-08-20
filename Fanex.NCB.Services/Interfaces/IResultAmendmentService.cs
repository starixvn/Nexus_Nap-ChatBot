namespace Fanex.NCB.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IResultAmendmentService
    {
        Task ResultAmendmentInternalCSDAsync();

        Task ResultAmendmentLicenseeAsync();
    }
}
