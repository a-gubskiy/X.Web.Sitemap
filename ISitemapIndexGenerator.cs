using System.Collections.Generic;
using System.IO;

namespace X.Web.Sitemap
{
    public interface ISitemapIndexGenerator
    {
        void GenerateSitemapIndex(List<SitemapInfo> sitemaps, DirectoryInfo targetDirectory, string targetSitemapFileName);
    }
}
