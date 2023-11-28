namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Extensions;

    public class GetMissingMessageDataCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public List<MissingMesssageInfo> Messages { get; set; } = new List<MissingMesssageInfo>();

        [SpParam(SqlParamName = "MessageList")]
        public string MessageListXml
        {
            get
            {
                return XmlHelper<MissingMesssageInfo>.Serialize(Messages);
            }
        }

        public override string GetSettingKey() => "NAP_Chatbot_MissingMessage_Sel";

        public override bool IsValid() => true;
    }
}
