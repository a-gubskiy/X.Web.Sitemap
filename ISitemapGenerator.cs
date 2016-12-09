using System.Collections.Generic;
using System.IO;

namespace X.Web.Sitemap
{
    public interface ISitemapGenerator
    {
        List<FileInfo> GenerateSitemaps(List<Url> urls, DirectoryInfo targetDirectory, string sitemapBaseFileNameWithoutExtension = "sitemap");
    }
}
