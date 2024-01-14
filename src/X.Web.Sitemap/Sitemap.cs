using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("X.Web.Sitemap.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace X.Web.Sitemap;

[PublicAPI]
[Serializable]
[XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public class Sitemap : List<Url>, ISitemap
{
    public static int DefaultMaxNumberOfUrlsPerSitemap { get; set; } = 5000;

    public Sitemap()
    {
    }

    public Sitemap(IEnumerable<Url> urls) => AddRange(urls);

    [PublicAPI]
    public static Sitemap Parse(string xml) => new SitemapSerializer().Deserialize(xml);

    [PublicAPI]
    public static bool TryParse(string xml, out Sitemap? sitemap)
    {
        try
        {
            sitemap = Parse(xml);
        }
        catch
        {
            sitemap = null;
        }

        return sitemap != null;
    }
}
