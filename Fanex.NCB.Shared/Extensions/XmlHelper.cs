namespace Fanex.NCB.Shared.Extensions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public static class XmlHelper<T>
    {
        private static readonly XmlSerializer serializer =
                new(typeof(List<T>), new XmlRootAttribute("root"));

        public static string Serialize(List<T> input)
        {
            using var stream = new StringWriter();
            serializer.Serialize(stream, input);
            return stream.ToString();
        }
    }
}