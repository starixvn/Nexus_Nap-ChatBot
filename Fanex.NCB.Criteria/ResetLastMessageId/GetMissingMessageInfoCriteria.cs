namespace Fanex.NCB.Criteria.ResetLastMessageId
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Models.ResetLastMessageId;
    using Fanex.NCB.Shared.Extensions;

    public class GetMissingMessageInfoCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public List<CurrentMaxMessageInfo> Messages { get; set; } = new List<CurrentMaxMessageInfo>();

        [SpParam(Ignored = true)]
        public List<MessageTypeInfo> MsgContentTypes { get; set; } = new List<MessageTypeInfo>();

        [SpParam(Ignored = true)]
        public List<MessageRequestId> Requests { get; set; } = new List<MessageRequestId>();

        [SpParam(SqlParamName = "MessageList")]
        public string MessageListXml
        {
            get
            {
                return XmlHelper<CurrentMaxMessageInfo>.Serialize(Messages);
            }
        }

        [SpParam(SqlParamName = "MsgContentsTypeList")]
        public string MsgContentsTypeListXml
        {
            get
            {
                return XmlHelper<MessageTypeInfo>.Serialize(MsgContentTypes);
            }
        }

        [SpParam(SqlParamName = "RequestList")]
        public string RequestListXml
        {
            get
            {
                return XmlHelper<MessageRequestId>.Serialize(Requests);
            }
        }

        public override string GetSettingKey() => "NAP_Chatbot_MissingMesssageInfo_Sel";

        public override bool IsValid() => true;
    }
}