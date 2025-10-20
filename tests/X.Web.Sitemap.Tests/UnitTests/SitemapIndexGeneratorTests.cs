using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class SitemapIndexGeneratorTests : IDisposable
    {
        private readonly string _tempDir;

        public SitemapIndexGeneratorTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        [Fact]
        public void GenerateSitemapIndex_StringOverload_WritesFile()
        {
            var info = new SitemapInfo(new Uri("http://example.com/s1.xml"), new DateTime(2020,1,1));
            var generator = new SitemapIndexGenerator();
            var fileName = "sitemapindex.xml";

            var index = generator.GenerateSitemapIndex(new[] { info }, _tempDir, fileName);

            Assert.NotNull(index);
            var path = Path.Combine(_tempDir, fileName);
            Assert.True(File.Exists(path));
            var content = File.ReadAllText(path);
            Assert.Contains("http://example.com/s1.xml", content);
        }
    }
}

