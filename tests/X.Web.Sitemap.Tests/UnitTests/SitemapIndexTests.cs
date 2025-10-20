using Xunit;
using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class SitemapIndexTests
    {
        [Fact]
        public void SerializeAndDeserialize_RoundTrip_PreservesData()
        {
            var info = new SitemapInfo(new Uri("http://example.com/sitemap1.xml"), new DateTime(2020,1,2));
            var index = new SitemapIndex(new[] { info });

            var serializer = new SitemapIndexSerializer();
            var xml = serializer.Serialize(index);

            Assert.False(string.IsNullOrWhiteSpace(xml));
            Assert.Contains("sitemapindex", xml);
            Assert.Contains("sitemap1.xml", xml);

            var deserialized = serializer.Deserialize(xml);
            Assert.NotNull(deserialized);
            Assert.Single(deserialized.Sitemaps);
            Assert.Equal(info.AbsolutePathToSitemap, deserialized.Sitemaps.First().AbsolutePathToSitemap);
            Assert.Equal(info.DateLastModified, deserialized.Sitemaps.First().DateLastModified);
        }

        [Fact]
        public void Parse_ExampleBadXml_TryParseReturnsFalse()
        {
            var bad = "<notvalid></notvalid>";
            var ok = SitemapIndex.TryParse(bad, out var sitemapIndex);

            Assert.False(ok);
            Assert.Null(sitemapIndex);
        }

        [Fact]
        public void Extension_ToStreamAndToXml_Work()
        {
            var info = new SitemapInfo(new Uri("http://example.com/sitemap1.xml"), new DateTime(2020,1,2));
            var index = new SitemapIndex(new[] { info });

            var xml = index.ToXml();
            Assert.False(string.IsNullOrWhiteSpace(xml));
            Assert.Contains("sitemapindex", xml);

            using var stream = index.ToStream();
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            Assert.Contains("sitemap1.xml", text);
        }
    }
}

