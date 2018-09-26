using System.Collections.Generic;
using System.IO;

namespace X.Web.Sitemap
{
    public class SitemapGenerator : ISitemapGenerator
    {
        private readonly ISerializedXmlSaver<Sitemap> _serializedXmlSaver;
        public const int MaxNumberOfUrlsPerSitemap = 50000;

        public SitemapGenerator()
        {
            _serializedXmlSaver = new SerializedXmlSaver<Sitemap>(new FileSystemWrapper());
        }

        internal SitemapGenerator(ISerializedXmlSaver<Sitemap> serializedXmlSaver)
        {
            _serializedXmlSaver = serializedXmlSaver;
        }

        public List<FileInfo> GenerateSitemaps(List<Url> urls, DirectoryInfo targetDirectory, string sitemapBaseFileNameWithoutExtension = "sitemap")
        {
            var sitemaps = BuildSitemaps(urls);

            var sitemapFileInfos = SaveSitemaps(targetDirectory, sitemapBaseFileNameWithoutExtension, sitemaps);

            return sitemapFileInfos;
        }

        private static List<Sitemap> BuildSitemaps(List<Url> urls)
        {
            var sitemaps = new List<Sitemap>();
            var sitemap = new Sitemap();
            var numberOfUrls = urls.Count;
            for (var i = 0; i < numberOfUrls; i++)
            {
                if (i%MaxNumberOfUrlsPerSitemap == 0)
                {
                    sitemap = new Sitemap();
                    sitemaps.Add(sitemap);
                }

                sitemap.Add(urls[i]);
            }
            return sitemaps;
        }


        private List<FileInfo> SaveSitemaps(DirectoryInfo targetDirectory, string sitemapBaseFileNameWithoutExtension, List<Sitemap> sitemaps)
        {
            var sitemapFileInfos = new List<FileInfo>();
            for (var i = 0; i < sitemaps.Count; i++)
            {
                var fileName = $"{sitemapBaseFileNameWithoutExtension}-00{i + 1}.xml";
                sitemapFileInfos.Add(_serializedXmlSaver.SerializeAndSave(sitemaps[i], targetDirectory, fileName));
            }
            return sitemapFileInfos;
        }
    }
}
