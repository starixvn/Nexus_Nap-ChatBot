namespace Fanex.NCB.AutoJobs.Core.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface ISettlementInternalLicenseeService
    {
        Task SettlementInternalCSDAsync();

        Task SettlementLicenseeAsync();
    }
}