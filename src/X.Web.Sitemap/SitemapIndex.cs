using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[Serializable]
[XmlRoot(ElementName = "sitemapindex", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public class SitemapIndex
{
    private SitemapIndex()
    {
        Sitemaps = new List<SitemapInfo>();
    }

    /// <summary>
    /// Creates a sitemap index which serializes to a sitemapindex element of a sitemap
    /// index file: https://www.sitemaps.org/protocol.html#index 
    /// </summary>
    /// <param name="sitemaps">A list of sitemap metadata to include in the sitemap index.</param>
    public SitemapIndex(IEnumerable<SitemapInfo> sitemaps)
    {
        Sitemaps = sitemaps.ToList();
    }

    [XmlElement("sitemap")]
    public List<SitemapInfo> Sitemaps { get; private set; }
    
    [PublicAPI]
    public static SitemapIndex Parse(string xml) => new SitemapIndexSerializer().Deserialize(xml);

    [PublicAPI]
    public static bool TryParse(string xml, out SitemapIndex? sitemapIndex)
    {
        try
        {
            sitemapIndex = Parse(xml);
        }
        catch
        {
            sitemapIndex = null;
        }

        return sitemapIndex != null;
    }
}