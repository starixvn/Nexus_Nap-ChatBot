namespace Fanex.NCB.Repository.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models;

    public interface ISpecialDeleteRepository
    {
        Task<(CommandResult, IEnumerable<NotificationMessage>)> ImportMessageAndGetNotification(ICriteria criteria);
    }
}