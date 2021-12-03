using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("X.Web.Sitemap.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace X.Web.Sitemap
{
    [PublicAPI]
    [Serializable]
    [XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap : List<Url>, ISitemap
    {
        private readonly ISitemapSerializer _sitemapSerializer;

        public static int DefaultMaxNumberOfUrlsPerSitemap = 5000;
        
        public int MaxNumberOfUrlsPerSitemap { get; set; }

        public Sitemap()
            : this(new SitemapSerializer())
        {
        }

        public Sitemap(ISitemapSerializer sitemapSerializer)
        {
            _sitemapSerializer = sitemapSerializer;
            MaxNumberOfUrlsPerSitemap = DefaultMaxNumberOfUrlsPerSitemap;
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
        
        public static Sitemap Parse(string xml) => SitemapSerializer.Parse<Sitemap>(xml);
        
        public static bool TryParse(string xml, out Sitemap sitemap) => SitemapSerializer.TryParse(xml, out sitemap);
    }
}
