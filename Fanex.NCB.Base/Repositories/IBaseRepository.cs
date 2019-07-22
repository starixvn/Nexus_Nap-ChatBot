namespace Fanex.NCB.Base.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Base.Models;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }
}