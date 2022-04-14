namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Shared.Models.ProblemAccounts;

    public interface IProblemAccountService
    {
        Task<IEnumerable<MessageSetting>> GetMessageSetting();
    }
}
