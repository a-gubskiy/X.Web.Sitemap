using System;
using System.Xml.Serialization;

namespace X.Web.Sitemap;

[Serializable]
public class SitemapInfo
{
    private SitemapInfo()
    {
        AbsolutePathToSitemap = "";
        DateLastModified = "";
    }

    /// <summary>
    /// Creates a SitemapInfo object which serializes to the "sitemap" element of a sitemap index
    /// file: https://www.sitemaps.org/protocol.html#index 
    /// </summary>
    /// <param name="absolutePathToSitemap">
    /// The full path to the sitemap (e.g. https://www.somewebsite.com/sitemaps/sitemap1.xml). Serializes
    /// to the "loc" element.
    /// </param>
    /// <param name="dateSitemapLastModified">
    /// The date the sitemap was last modified/created. Serializes to the "lostmod" element.
    /// </param>
    public SitemapInfo(Uri absolutePathToSitemap, DateTime? dateSitemapLastModified = null)
    {
        AbsolutePathToSitemap = absolutePathToSitemap.ToString();
        DateLastModified = dateSitemapLastModified?.ToString("yyyy-MM-dd") ?? string.Empty;
    }

    /// <summary>
    /// The full path to the sitemap (e.g. https://www.somewebsite.com/sitemaps/sitemap1.xml).
    /// Serializes to the "loc" element.
    /// </summary>
    [XmlElement("loc")]
    public string AbsolutePathToSitemap { get; set; }

    /// <summary>
    /// The date the sitemap was last modified/created. Serializes to the "lostmod" element.
    /// </summary>
    [XmlElement("lastmod")]
    public string DateLastModified{ get; set; }
}