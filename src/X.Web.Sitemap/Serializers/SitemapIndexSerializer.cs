using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
public interface ISitemapIndexSerializer
{
    string Serialize(SitemapIndex sitemap);
    
    SitemapIndex Deserialize(string xml);
}

public class SitemapIndexSerializer : ISitemapIndexSerializer
{
    private readonly XmlSerializer _serializer = new(typeof(SitemapIndex));

    public string Serialize(SitemapIndex sitemapIndex)
    {
        if (sitemapIndex == null)
        {
            throw new ArgumentNullException(nameof(sitemapIndex));
        }
        
        var xml = "";

        using (var writer = new StringWriterUtf8())
        {
            _serializer.Serialize(writer, sitemapIndex);
            
            xml = writer.ToString();
        }

        return xml;
    }

    public SitemapIndex Deserialize(string xml)
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

            return (SitemapIndex)obj;
        }
    }
}