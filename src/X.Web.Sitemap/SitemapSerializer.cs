using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace X.Web.Sitemap;

public interface ISitemapSerializer
{
    string Serialize(ISitemap sitemap);
}

public class SitemapSerializer : ISitemapSerializer
{
    public string Serialize(ISitemap sitemap)
    {
        if (sitemap == null)
        {
            throw new ArgumentNullException(nameof(sitemap));
        }

        var serializer = new XmlSerializer(typeof(Sitemap));
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");

        using (var writer = new StringWriterUtf8())
        {
            serializer.Serialize(writer, sitemap, namespaces);
            return writer.ToString();
        }
    }

    public static Sitemap Deserialize(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException();
        }
        
        using (TextReader textReader = new StringReader(xml))
        {
            var serializer = new XmlSerializer(typeof(Sitemap));
            var obj = serializer.Deserialize(textReader);

            if (obj is null)
            {
                throw new XmlException();
            }

            return (Sitemap)obj;
        }
    }
}