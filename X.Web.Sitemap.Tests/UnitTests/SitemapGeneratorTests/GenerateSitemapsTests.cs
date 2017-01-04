using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using NSubstitute;

namespace X.Web.Sitemap.Tests.UnitTests.SitemapGeneratorTests
{
    [TestFixture]
    public class GenerateSitemapsTests
    {
        private SitemapGenerator _sitemapGenerator;
        private ISerializedXmlSaver<Sitemap> _sitemapSerializerMock;

        [SetUp]
        public void SetUp()
        {
            _sitemapSerializerMock = Substitute.For<ISerializedXmlSaver<Sitemap>>();
            _sitemapGenerator = new SitemapGenerator(_sitemapSerializerMock);
        }

        [Test]
        public void It_Only_Saves_One_Sitemap_If_There_Are_Less_Than_50001_Urls()
        {
            //--arrange
            var maxNumberOfUrlsForOneSitemap = SitemapGenerator.MaxNumberOfUrlsPerSitemap;
            var urls = new List<Url>(maxNumberOfUrlsForOneSitemap);
            for (var i = 0; i < maxNumberOfUrlsForOneSitemap; i++)
            {
                urls.Add(new Url());
            }

            //--act
            _sitemapGenerator.GenerateSitemaps(urls, new DirectoryInfo("x"), "file");

            //--assert
            _sitemapSerializerMock
                .Received(1)
                .SerializeAndSave(Arg.Is<Sitemap>(x => x.Count == maxNumberOfUrlsForOneSitemap), Arg.Any<DirectoryInfo>(), Arg.Any<string>());
        }

        [Test]
        public void It_Saves_Two_Sitemaps_If_There_Are_More_Than_50000_Urls_But_Less_Than_100001_And_It_Names_The_Files_With_A_Three_Digit_Suffix_Incrementing_For_Each_One()
        {
            //--arrange
            var enoughForTwoSitemaps = SitemapGenerator.MaxNumberOfUrlsPerSitemap + 1;
            var urls = new List<Url>(enoughForTwoSitemaps);
            for (var i = 0; i < enoughForTwoSitemaps; i++)
            {
                urls.Add(new Url());
            }
            var fileName = "file";
            var directory = new DirectoryInfo("x");

            //--act
            _sitemapGenerator.GenerateSitemaps(urls, directory, fileName);

            //--assert
            _sitemapSerializerMock
                .Received(1)
                .SerializeAndSave(Arg.Is<Sitemap>(x => x.Count == SitemapGenerator.MaxNumberOfUrlsPerSitemap), Arg.Is<DirectoryInfo>(x => x == directory), Arg.Is<string>(x => x == "file-001.xml"));

            _sitemapSerializerMock
                 .Received(1)
                .SerializeAndSave(Arg.Is<Sitemap>(x => x.Count == 1), Arg.Is<DirectoryInfo>(x => x == directory), Arg.Is<string>(x => x == "file-002.xml"));
        }
    }
}
