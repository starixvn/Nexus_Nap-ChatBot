using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Fanex.NCB.Shared.Extensions
{
    public static class XmlHelper
    {
        public static string Serialize<T>(IEnumerable<T> input)
        {
            var serializer = new XmlSerializer(typeof(List<T>),
                                   new XmlRootAttribute("root"));

            var stream = new StringWriter();
            serializer.Serialize(stream, input);
            return stream.ToString();
        }
    }
}