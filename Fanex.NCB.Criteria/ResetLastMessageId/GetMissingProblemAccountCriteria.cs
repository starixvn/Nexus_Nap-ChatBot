namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using Fanex.Data.Repository;

    public class GetMissingProblemAccountCriteria : CriteriaBase
    {
        public int FromLogId { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_MissingProblemAccounts_Sel";

        public override bool IsValid() => true;
    }
}
