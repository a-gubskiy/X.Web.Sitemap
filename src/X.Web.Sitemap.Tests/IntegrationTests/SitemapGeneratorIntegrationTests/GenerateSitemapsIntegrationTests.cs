using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using NSubstitute;
using NUnit.Framework;

namespace X.Web.Sitemap.Tests.IntegrationTests.SitemapGeneratorIntegrationTests
{
    [TestFixture]
    public class GenerateSitemapsIntegrationTests
    {
        private SitemapGenerator _sitemapGenerator;
        private readonly string _sitemapLocation = Path.GetTempPath();

        [SetUp]
        public void SetUp()
        {
            _sitemapGenerator = new SitemapGenerator();
        }

        [Test]
        public void It_Only_Saves_One_Sitemap_If_There_Are_Less_Than_50001_Urls()
        {
            //--arrange
            var maxNumberOfUrlsForOneSitemap = SitemapGenerator.MaxNumberOfUrlsPerSitemap;
            var urls = new List<Url>(maxNumberOfUrlsForOneSitemap);
            var now = DateTime.UtcNow;
            for (var i = 0; i < maxNumberOfUrlsForOneSitemap; i++)
            {
                urls.Add(Url.CreateUrl("https://example.com/" + i, now));
            }

            //--act
            _sitemapGenerator.GenerateSitemaps(urls, new DirectoryInfo(_sitemapLocation), "sitemap_from_test_1");

            //--assert
            //--go look in the {sitemapLocation} directory!
        }

        [Test]
        public void It_Saves_Two_Sitemaps_If_There_Are_More_Than_50000_Urls_But_Less_Than_100001_And_It_Names_The_Files_With_A_Three_Digit_Suffix_Incrementing_For_Each_One()
        {
            //--arrange
            var enoughUrlsForTwoSitemaps = SitemapGenerator.MaxNumberOfUrlsPerSitemap + 1;
            var urls = new List<Url>(enoughUrlsForTwoSitemaps);
            var now = DateTime.UtcNow;
            for (var i = 0; i < enoughUrlsForTwoSitemaps; i++)
            {
                urls.Add(Url.CreateUrl("https://example.com/" + i, now));
            }

            //--act
            _sitemapGenerator.GenerateSitemaps(urls, new DirectoryInfo(_sitemapLocation), "sitemap_from_test_2");

            //--assert
            //--go look for 2 sitemaps in the {sitemapLocation} directory!
        }
    }
}
