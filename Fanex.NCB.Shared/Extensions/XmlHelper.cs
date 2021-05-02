namespace Fanex.NCB.Shared.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Fanex.NCB.Shared.Models.SiteMessage;

    public static class XmlHelper
    {
        private static readonly XmlSerializer serializer = 
                new XmlSerializer(typeof(List<MessageHistory>), new XmlRootAttribute("root"));
        public static string Serialize<T>(List<T> input)
        {
            using var stream = new StringWriter();
            serializer.Serialize(stream, input);
            return stream.ToString();
        }
    }
}