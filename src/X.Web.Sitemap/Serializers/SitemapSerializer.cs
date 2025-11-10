using System;
using System.Collections.Generic;
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

        string xml;

        using (var writer = new StringWriterUtf8())
        {
            _serializer.Serialize(writer, sitemap);
            
            xml = writer.ToString();
        }

        return XmlPostProcessing(xml);
    }

    private static string XmlPostProcessing(string xml)
    {
        // Post-process generated XML to remove xsi:nil="true" for <changefreq> elements.
        // This avoids changing the Url class while ensuring the output conforms to the
        // Sitemaps protocol (no nil attributes for optional elements).
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var nodes = doc.GetElementsByTagName("changefreq");
            
            const string xsiNs = "http://www.w3.org/2001/XMLSchema-instance";

            // Ensure root has the sitemap default namespace and remove only the xsi namespace
            // declarations that are no longer needed (e.g. xmlns:xsi and xsi:schemaLocation).
            var root = doc.DocumentElement;
            
            const string sitemapNs = "http://www.sitemaps.org/schemas/sitemap/0.9";

            if (root is not null)
            {
                // Ensure default xmlns is present and correct
                root.SetAttribute("xmlns", sitemapNs);

                // Remove xmlns:xsi if present
                var xmlnsXsi = root.GetAttributeNode("xmlns:xsi");
                
                if (xmlnsXsi is not null)
                {
                    root.RemoveAttributeNode(xmlnsXsi);
                }

                // Remove xsi:schemaLocation if present
                var schemaLoc = root.GetAttributeNode("schemaLocation", xsiNs);
                
                if (schemaLoc is not null)
                {
                    root.RemoveAttributeNode(schemaLoc);
                }
            }

            // Collect nodes first to avoid modifying the live XmlNodeList during iteration
            var list = new List<XmlElement>();
            
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

            using var writer = new StringWriterUtf8();
            
            doc.Save(writer);
            
            return writer.ToString();
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

        using TextReader textReader = new StringReader(xml);
        
        var obj = _serializer.Deserialize(textReader);

        if (obj is null)
        {
            throw new XmlException();
        }

        return (Sitemap)obj;
    }
}