using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace X.Web.Sitemap.Tests
{
    [TestFixture]
    public class IssueTest
    {
        class Page
        {
            public DateTime LastUpdated { get; set; }
            public string Url { get; set; }
        }

        [Test]
        public void Test()
        {
            var pages = new List<Page>();

            for (int i = 0; i < 1000; i++)
            {
                pages.Add(new Page
                {
                    LastUpdated = DateTime.Now.AddDays(-i).AddHours(i),
                    Url = $"https://example.com/{i}/page_{i}.html"
                });
            }


            var sitemap = new Sitemap();
            sitemap.AddRange(pages.Select(page => new Url {Location = page.Url, TimeStamp = page.LastUpdated}));

            var sitemapGenerator = new X.Web.Sitemap.SitemapGenerator();
            var targetSitemapDirectory = new DirectoryInfo("/Users/andrew/pub/sitemap");
            sitemapGenerator.GenerateSitemaps(sitemap, targetSitemapDirectory);

            // generate one or more sitemaps (depending on the number of URLs) in the designated location.
            var fileInfoForGeneratedSitemaps = sitemapGenerator.GenerateSitemaps(sitemap, targetSitemapDirectory);

            var sitemapInfos = new List<SitemapInfo>();
            var dateSitemapWasUpdated = pages.Max(q => q.LastUpdated);

            foreach (var fileInfo in fileInfoForGeneratedSitemaps)
            {
                var url = $"https://example.com/content/sitemaps/{fileInfo.Name}";
                var uriToSitemap = new Uri(url);
                sitemapInfos.Add(new SitemapInfo(uriToSitemap, dateSitemapWasUpdated));
            }

            // now generate the sitemap index file which has a reference to all of the sitemaps that were generated. 
            var sitemapIndexGenerator = new SitemapIndexGenerator();
            sitemapIndexGenerator.GenerateSitemapIndex(sitemapInfos, targetSitemapDirectory, "sitemap-index.xml");
        }
    }
}