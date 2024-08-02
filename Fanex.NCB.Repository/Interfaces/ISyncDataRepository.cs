namespace Fanex.NCB.Repository.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models.NapUserControl;
    using Fanex.NCB.Shared.Models.OIC;

    public interface ISyncDataRepository
    {
        Task<NapData> FetchDataFromNapUserControl(ICriteria criteria);
    }
}