namespace Fanex.NCB.Criteria.ResultAmendmentInternalCSD
{
    using System;
    using Fanex.Data.Repository;

    public class GetMessageResultAmendmentCsdCriteria : CriteriaBase
    {
        public DateTime LastModifiedTime { get; set; }

        public DateTime? NewLastModifiedTime { get; set; }

        public override string GetSettingKey() => "NAP_Chatbot_ResultAmendment_InternalCSD_MessageSel";

        public override bool IsValid() => true;
    }
}
