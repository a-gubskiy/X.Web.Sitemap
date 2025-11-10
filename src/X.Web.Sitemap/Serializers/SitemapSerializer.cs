using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace X.Web.Sitemap.Serializers;

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
        _serializer = CreateSerializer();
    }

    private static XmlSerializer CreateSerializer()
    {
        return new XmlSerializer(typeof(Sitemap));
    }

    public string Serialize(ISitemap sitemap)
    {
        if (sitemap is null)
        {
            throw new ArgumentNullException(nameof(sitemap));
        }

        var xml = string.Empty;

        using (var writer = new StringWriterUtf8())
        {
            _serializer.Serialize(writer, sitemap);
            xml = writer.ToString();
        }

        // Post-process generated XML to remove xsi:nil="true" for <changefreq> elements.
        // This avoids changing the Url class while ensuring the output conforms to the
        // Sitemaps protocol (no nil attributes for optional elements).
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var nodes = doc.GetElementsByTagName("changefreq");
            var xsiNs = "http://www.w3.org/2001/XMLSchema-instance";

            // Collect nodes first to avoid modifying the live XmlNodeList during iteration
            var list = new System.Collections.Generic.List<XmlElement>();
            foreach (XmlNode node in nodes)
            {
                if (node is XmlElement el)
                {
                    list.Add(el);
                }
            }

            foreach (var el in list)
            {
                var attr = el.GetAttributeNode("nil", xsiNs);
                
                if (attr != null && string.Equals(attr.Value, "true", StringComparison.OrdinalIgnoreCase))
                {
                    // remove the entire element to avoid deserializing an empty value into the enum
                    var parent = el.ParentNode;
                    parent?.RemoveChild(el);
                }
            }

            using var sw = new StringWriterUtf8();
            doc.Save(sw);
            return sw.ToString();
        }
        catch
        {
            // If anything goes wrong in post-processing, fall back to the original XML
            return xml;
        }
    }

    public Sitemap Deserialize(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException(nameof(xml));
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