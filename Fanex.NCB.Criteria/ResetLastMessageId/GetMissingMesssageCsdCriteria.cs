namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using Fanex.Data.Repository;

    public class GetMissingMesssageCsdCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "NAP_Chatbot_AbnormalBets_MissingMesssageInfo_Sel";

        public override bool IsValid() => true;
    }
}