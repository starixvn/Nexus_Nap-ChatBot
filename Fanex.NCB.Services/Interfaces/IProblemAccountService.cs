namespace Fanex.NCB.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.ProblemAccounts;

    public interface IProblemAccountService
    {
        Task<MessageSetting> GetMessageSettingAsync();

        Task<(IEnumerable<ProblemAccountInfo>, IEnumerable<short>)> GetProblemAccountAsync();

        Task<CommandResult> CreateFromProblemAccountAsync();
    }
}
