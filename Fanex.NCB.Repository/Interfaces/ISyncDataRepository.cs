namespace Fanex.NCB.Repository.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models.NapUserControl;

    public interface ISyncDataRepository
    {
        Task<NapData> FetchDataFromNapUserControl(ICriteria criteria);
    }
}