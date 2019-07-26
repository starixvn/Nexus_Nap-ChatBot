namespace Fanex.NCB.Core.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Core.Models;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }
}