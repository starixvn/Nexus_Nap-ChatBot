namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.ResetLastMessageId;

    public class GetMissingMessageInfoCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public List<MessageInfo> Messages { get; set; } = new List<MessageInfo>();

        [SpParam(Ignored = true)]
        public List<MessageTypeInfo> MsgContentTypes { get; set; } = new List<MessageTypeInfo>();

        [SpParam(Ignored = true)]
        public List<MessageRequestId> Requests { get; set; } = new List<MessageRequestId>();

        [SpParam(SqlParamName = "MessageList")]
        public string MessageListXml
        {
            get
            {
                var xml = new StringBuilder();
                xml.Append("<root>");
                foreach (MessageInfo message in Messages)
                {
                    xml.AppendFormat("<r JobIdentify='{0}' FromMessageId='{1}' ToMessageId='{2}' PostTime='{3}'/>",
                        message.JobIdentify,
                        message.FromMessageId,
                        message.ToMessageId,
                        message.PostTime);
                }
                xml.Append("</root>");
                return xml.ToString();
            }
        }

        [SpParam(SqlParamName = "MsgContentsTypeList")]
        public string MsgContentsTypeListXml
        {
            get
            {
                var xml = new StringBuilder();
                xml.Append("<root>");
                foreach (MessageTypeInfo msgContentType in MsgContentTypes)
                {
                    xml.AppendFormat("<r JobIdentify='{0}' MsgContentsType='{1}'/>",
                        msgContentType.JobIdentify,
                        msgContentType.MsgContentsType);
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

        public override string GetSettingKey() => "NAP_Chatbot_MissingMesssageInfo_Sel";

        public override bool IsValid() => true;
    }
}
