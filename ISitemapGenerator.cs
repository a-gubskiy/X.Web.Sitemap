using System.Collections.Generic;

namespace X.Web.Sitemap
{
    public interface ISitemapGenerator
    {
        void GenerateSitemaps(List<Url> urls);
    }
}
