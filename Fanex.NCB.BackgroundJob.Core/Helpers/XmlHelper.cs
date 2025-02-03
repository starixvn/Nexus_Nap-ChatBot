namespace Fanex.NCB.AutoJobs.Core.Helpers;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

[ExcludeFromCodeCoverage(Justification = "Helpers")]
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