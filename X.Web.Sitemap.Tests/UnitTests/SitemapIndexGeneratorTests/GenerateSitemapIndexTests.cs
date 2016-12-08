using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NSubstituteAutoMocker;
using NUnit.Framework;

namespace X.Web.Sitemap.Tests.UnitTests.SitemapIndexGeneratorTests
{
    [TestFixture]
    public class GenerateSitemapIndexTests
    {
        private NSubstituteAutoMocker<SitemapIndexGenerator> _autoMocker;
            
        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<SitemapIndexGenerator>();
        }

        [Test]
        public void It_Saves_A_Generated_Sitemap_Index_File_From_The_Specified_Sitemaps()
        {
            //--arrange
            var sitemaps = new List<SitemapInfo>
            {
                new SitemapInfo(new Uri("https://example.com"), DateTime.UtcNow),
                new SitemapInfo(new Uri("https://example2.com"), DateTime.UtcNow.AddDays(-1))
            };
            var expectedDirectory = new DirectoryInfo(@"C:\temp\sitemaptests\");
            var expectedFilename = "testSitemapIndex1.xml";

            //--act
            _autoMocker.ClassUnderTest.GenerateSitemapIndex(sitemaps, expectedDirectory, expectedFilename);

            //--assert
            _autoMocker.Get<ISerializedXmlSaver<SitemapIndex>>().Received().SerializeAndSave(
                Arg.Is<SitemapIndex>(x => AssertCorrectSitemapIndexWasSerialized(sitemaps, x)),
                Arg.Is<DirectoryInfo>(x => x == expectedDirectory),
                Arg.Is<string>(x => x == expectedFilename));
        }

        private bool AssertCorrectSitemapIndexWasSerialized(IEnumerable<SitemapInfo> expectedSitemaps, SitemapIndex actualSitemapIndex)
        {
            foreach (var expectedSitemap in expectedSitemaps)
            {
                if (!actualSitemapIndex.Sitemaps.Contains(expectedSitemap))
                {
                    Assert.Fail("Received a call to .SerializeAndSave, but at least one of the expected sitemapInfos was missing.");
                }
            }

            return true;
        }
    }
}
