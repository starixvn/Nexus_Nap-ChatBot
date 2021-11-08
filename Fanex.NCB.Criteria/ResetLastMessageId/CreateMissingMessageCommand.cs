namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.ResetLastMessageId;

    public class CreateMissingMessageCommand : NonQueryCommand
    {
        [SpParam(Ignored = true)]
        public List<CurrentMaxMessageInfo> Messages { get; set; } = new List<CurrentMaxMessageInfo>();

        [SpParam(Ignored = true)]
        public List<MessageRequestId> Requests { get; set; } = new List<MessageRequestId>();

        public string Environment { get; set; } = "UAT";

        [SpParam(SqlParamName = "MessageList")]
        public string MessageListXml
        {
            get
            {
                var xml = new StringBuilder();
                xml.Append("<root>");
                foreach (CurrentMaxMessageInfo message in Messages)
                {
                    xml.AppendFormat("<r JobIdentify='{0}' MessageId='{1}'/>",
                        message.JobIdentify,
                        message.MessageId);
                }
                xml.Append("</root>");
                return xml.ToString();
            }
        }

        [SpParam(SqlParamName = "RequestList")]
        public string RequestListXml
        {
            get
            {
                var xml = new StringBuilder();
                xml.Append("<root>");
                foreach (MessageRequestId request in Requests)
                {
                    xml.AppendFormat("<r MessageId='{0}' RequestId='{1}'/>",
                        request.MessageId,
                        request.RequestId);
                }
                xml.Append("</root>");
                return xml.ToString();
            }
        }

        public override string GetSettingKey() => "NAP_Chatbot_MissingMesssage_Ins";

        public override bool IsValid() => true;
    }
}
