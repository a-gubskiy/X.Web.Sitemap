using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
public interface ISitemapSerializer
{
    string Serialize(ISitemap sitemap);
    
    Sitemap Deserialize(string xml);
}

public class SitemapSerializer : ISitemapSerializer
{
    private readonly XmlSerializer _serializer;

    public SitemapSerializer()
    {
        _serializer = new XmlSerializer(typeof(Sitemap));
    }

    public string Serialize(ISitemap sitemap)
    {
        if (sitemap == null)
        {
            throw new ArgumentNullException(nameof(sitemap));
        }

        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");

        using (var writer = new StringWriterUtf8())
        {
            _serializer.Serialize(writer, sitemap, namespaces);

            return writer.ToString();
        }
    }

    public Sitemap Deserialize(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException();
        }

        using (TextReader textReader = new StringReader(xml))
        {
            var obj = _serializer.Deserialize(textReader);

            if (obj is null)
            {
                throw new XmlException();
            }

            return (Sitemap)obj;
        }
    }
}