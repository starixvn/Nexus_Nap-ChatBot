namespace Fanex.NCB.Criteria.ProblemAccount
{
    using System.Collections.Generic;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Models.ProblemAccounts;

    public class CreateFromProblemAccountCriteria : BaseCommand
    {
        public string Message { get; set; }

        public string CategoryInfo { get; set; }

        public override string GetSettingKey() => "NCB_ProblemAccount_Import";

        public override bool IsValid() => true;
    }
}
