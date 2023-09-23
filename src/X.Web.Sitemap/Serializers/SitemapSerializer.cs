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

        var settings = new XmlWriterSettings { Indent = true };

        using var writer = new StringWriterUtf8();
        {
            using (var xmlWriter = XmlWriter.Create(writer, settings))
            {
                _serializer.Serialize(xmlWriter, sitemap, namespaces);
            }
        }

        var xml = writer.ToString();

        // Hack for #39. Should be fixed in 
        xml = xml.Replace("<priority>1</priority>", "<priority>1.0</priority>");
        
        return xml;
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