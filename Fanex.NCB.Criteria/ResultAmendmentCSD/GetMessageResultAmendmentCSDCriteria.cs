namespace Fanex.NCB.Criteria.ResultAmendmentCSD
{
    using System;
    using Fanex.Data.Repository;

    public class GetMessageResultAmendmentCsdCriteria : CriteriaBase
    {
        public DateTime LastModifiedTime { get; set; }

        public DateTime NewLastModifiedTime { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_Settlement_InternalCSD_EventInfoSel";

        public override bool IsValid() => true;
    }
}
