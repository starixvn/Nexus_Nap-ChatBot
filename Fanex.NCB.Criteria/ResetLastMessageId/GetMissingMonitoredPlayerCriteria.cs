namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using Fanex.Data.Repository;

    public class GetMissingMonitoredPlayerCriteria : CriteriaBase
    {
        public int FromLogId { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_MissingMonitoredPlayer_Sel";

        public override bool IsValid() => true;
    }
}
