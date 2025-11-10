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
        _serializer = new XmlSerializer(typeof(Sitemap));
    }

    public string Serialize(ISitemap sitemap)
    {
        if (sitemap is null)
        {
            throw new ArgumentNullException(nameof(sitemap));
        }

        string xml;

        var settings = new XmlWriterSettings { Indent = true };

        using (var writer = new StringWriterUtf8())
        {
            using (var xmlWriter = XmlWriter.Create(writer, settings))
            {
                var namespaces = new XmlSerializerNamespaces();
                // set default namespace to sitemap protocol
                namespaces.Add(string.Empty, "http://www.sitemaps.org/schemas/sitemap/0.9");

                _serializer.Serialize(xmlWriter, sitemap, namespaces);
            }

            xml = writer.ToString();
        }

        return XmlPostProcessing(xml);
    }

    private static string XmlPostProcessing(string xml)
    {
        // Post-process generated XML to remove xsi:nil="true" for <changefreq> elements.
        // This avoids changing the Url class while ensuring the output conforms to the
        // Sitemaps protocol (no nil attributes for optional elements).

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

        // Normalize priority values: ensure integer values serialize as one decimal (e.g. 1 -> 1.0)
        var priorityNodes = doc.GetElementsByTagName("priority");
        var priorityList = new List<XmlElement>();

        foreach (XmlNode node in priorityNodes)
        {
            if (node is XmlElement el)
            {
                priorityList.Add(el);
            }
        }

        foreach (var p in priorityList)
        {
            var text = p.InnerText?.Trim() ?? string.Empty;

            // If the value is an integer (no decimal point) and a valid number, append .0
            if (!string.IsNullOrEmpty(text) && !text.Contains(".") && double.TryParse(text, out _))
            {
                p.InnerText = text + ".0";
            }
        }

        using var writer = new StringWriterUtf8();

        doc.Save(writer);

        return writer.ToString();
    }

    public Sitemap Deserialize(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException("XML string cannot be null or whitespace.", nameof(xml));
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