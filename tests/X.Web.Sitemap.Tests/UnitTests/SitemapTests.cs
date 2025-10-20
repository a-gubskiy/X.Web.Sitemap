using System;
using System.IO;
using Xunit;
using X.Web.Sitemap;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class SitemapTests
    {
        private static string ReadExampleXml()
        {
            var baseDir = AppContext.BaseDirectory;
            var path = Path.Combine(baseDir, "Data", "example.xml");
            if (!File.Exists(path))
            {
                // Try relative path fallback
                path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "example.xml");
            }

            return File.ReadAllText(path);
        }

        [Fact]
        public void Parse_ExampleXml_ProducesSitemapWithItems()
        {
            var xml = ReadExampleXml();
            var sitemap = Sitemap.Parse(xml);

            Assert.NotNull(sitemap);
            Assert.True(sitemap.Count >= 1);
            Assert.Contains(sitemap, u => u.Location.Contains("example.com"));
        }

        [Fact]
        public void TryParse_InvalidXml_ReturnsFalse()
        {
            var xml = "<notvalid></notvalid>";
            var result = Sitemap.TryParse(xml, out var sitemap);

            Assert.False(result);
            Assert.Null(sitemap);
        }
    }
}

