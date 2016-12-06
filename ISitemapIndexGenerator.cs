using System.Collections.Generic;

namespace X.Web.Sitemap
{
    public interface ISitemapIndexGenerator
    {
        void GenerateSitemapIndex(List<SitemapInfo> sitemaps);
    }
}
