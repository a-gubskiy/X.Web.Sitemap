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
        
        using var writer = new StringWriterUtf8();
        using var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true });

        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, "http://www.sitemaps.org/schemas/sitemap/0.9");

        _serializer.Serialize(xmlWriter, sitemap, namespaces);

        xmlWriter.Close();

        return XmlPostProcessing(writer.ToString());
    }

    private static string XmlPostProcessing(string xml)
    {
        const string xsiNs = "http://www.w3.org/2001/XMLSchema-instance";
        const string sitemapNs = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var doc = new XmlDocument();
        doc.LoadXml(xml);

        // Clean up root namespace declarations
        if (doc.DocumentElement is not null)
        {
            doc.DocumentElement.SetAttribute("xmlns", sitemapNs);
            doc.DocumentElement.RemoveAttribute("xmlns:xsi");
            doc.DocumentElement.RemoveAttribute("schemaLocation", xsiNs);
        }

        // Remove changefreq elements with xsi:nil="true"
        RemoveNilElements(doc, "changefreq", xsiNs);

        // Normalize priority values (1 -> 1.0)
        NormalizePriorityValues(doc);

        using var writer = new StringWriterUtf8();
        doc.Save(writer);
        return writer.ToString();
    }

    private static void RemoveNilElements(XmlDocument doc, string tagName, string xsiNs)
    {
        var elementsToRemove = new List<XmlElement>();

        var elements = doc.GetElementsByTagName(tagName);

        foreach (XmlNode node in elements)
        {
            if (node is not XmlElement xmlElement)
            {
                continue;
            }

            var attributeNode = xmlElement.GetAttributeNode("nil", xsiNs);

            if (attributeNode is null)
            {
                continue;
            }

            if (attributeNode.Value.Equals("true", StringComparison.OrdinalIgnoreCase) != true)
            {
                continue;
            }

            elementsToRemove.Add(xmlElement);
        }

        foreach (var element in elementsToRemove)
        {
            element.ParentNode?.RemoveChild(element);
        }
    }

    private static void NormalizePriorityValues(XmlDocument doc)
    {
        foreach (XmlNode node in doc.GetElementsByTagName("priority"))
        {
            if (node is not XmlElement el)
            {
                continue;
            }

            var text = el.InnerText?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(text))
            {
                continue;
            }

            if (!text.Contains(".") && double.TryParse(text, out _))
            {
                el.InnerText = text + ".0";
            }
        }
    }

    public Sitemap Deserialize(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentNullException(nameof(xml));
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