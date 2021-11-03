namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetNewPreviousMaxMessageCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public List<int> Messages { get; set; } = new List<int>();

        [SpParam(SqlParamName = "MessageList")]
        public string MessageListXml { 
            get {
                var xml = new StringBuilder();
                xml.Append("<root>");
                foreach (var message in Messages)
                {
                    xml.AppendFormat("<r MessageId='{0}'/>", message);
                }
                xml.Append("</root>");
                return xml.ToString();
            } 
        }

        public override string GetSettingKey() => "NAP_Chatbot_MaxMessageIdAndPostTime_Sel";

        public override bool IsValid() => true;
    }
}
