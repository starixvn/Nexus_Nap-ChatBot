namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System;
    using Fanex.Data.Repository;

    public class GetMissingMesssageCsdCriteria : CriteriaBase
    {
        public int FromMessageId { get; set; }

        public int ToMessageId { get; set; }

        public DateTime PostTime { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_AbnormalBets_MissingMesssageInfo_Sel";

        public override bool IsValid() => true;
    }
}
