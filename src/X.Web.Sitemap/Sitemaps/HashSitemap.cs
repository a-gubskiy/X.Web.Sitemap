using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("X.Web.Sitemap.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace X.Web.Sitemap;

[PublicAPI]
[Serializable]
[XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public class HashSitemap : HashSet<Url>, ISitemapBase
{
    private readonly ISitemapSerializer _sitemapSerializer;
        
    public int MaxNumberOfUrlsPerSitemap { get; set; }

    public HashSitemap()
        : this(new SitemapSerializer())
    {
    }

    public HashSitemap(ISitemapSerializer sitemapSerializer)
    {
        _sitemapSerializer = sitemapSerializer;
        MaxNumberOfUrlsPerSitemap = SitemapSerializer.DefaultMaxNumberOfUrlsPerSitemap;
    }

    public virtual string ToXml() => _sitemapSerializer.ToXml(this);

    public virtual Task<bool> SaveAsync(string path) => _sitemapSerializer.SaveAsync(path, this);

    public virtual bool Save(string path) => _sitemapSerializer.Save(path, this);

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public virtual bool SaveToDirectory(string directory) => _sitemapSerializer.SaveToDirectory(directory, this);
        
    public static HashSitemap Parse(string xml) => SitemapSerializer.Parse<HashSitemap>(xml);
        
    public static bool TryParse(string xml, out HashSitemap sitemap) => SitemapSerializer.TryParse(xml, out sitemap);
}