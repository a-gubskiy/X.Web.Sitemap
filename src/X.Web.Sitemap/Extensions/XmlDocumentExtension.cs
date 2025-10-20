using System.IO;
using System.Xml;

namespace X.Web.Sitemap.Extensions;

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