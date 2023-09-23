using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace X.Web.Sitemap.Extensions;

[PublicAPI]
public static class XmlDocumentExtension
{
    public static string ToXml(this XmlDocument document)
    {
        using (var writer = new StringWriter())
        {
            document.Save(writer);
            return writer.ToString();
        }
    }
}