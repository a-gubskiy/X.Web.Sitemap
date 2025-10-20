using Xunit;
using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class SitemapExtensionTests : IDisposable
    {
        private readonly string _tempDir;

        public SitemapExtensionTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true);
            }
            catch { }
        }

        [Fact]
        public void ToXml_And_ToStream_Work()
        {
            var sitemap = new Sitemap { Url.CreateUrl("http://example.com/page1") };

            var xml = ((ISitemap)sitemap).ToXml();
            Assert.Contains("example.com/page1", xml);

            using var stream = ((ISitemap)sitemap).ToStream();
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            Assert.Contains("example.com/page1", text);
        }

        [Fact]
        public void Save_WritesFile_ReturnsTrue()
        {
            var sitemap = new Sitemap { Url.CreateUrl("http://example.com/page2") };
            var path = Path.Combine(_tempDir, "out.xml");

            var ok = ((ISitemap)sitemap).Save(path);

            Assert.True(ok);
            Assert.True(File.Exists(path));
            Assert.Contains("example.com/page2", File.ReadAllText(path));
        }

        [Fact]
        public async Task SaveAsync_WritesFile_ReturnsTrue()
        {
            var sitemap = new Sitemap { Url.CreateUrl("http://example.com/page3") };
            var path = Path.Combine(_tempDir, "out-async.xml");

            var ok = await ((ISitemap)sitemap).SaveAsync(path);

            Assert.True(ok);
            Assert.True(File.Exists(path));
            Assert.Contains("example.com/page3", File.ReadAllText(path));
        }

        [Fact]
        public void SaveToDirectory_WritesSitemapFiles()
        {
            var sitemap = new Sitemap { Url.CreateUrl("http://example.com/page4") };

            var ok = ((ISitemap)sitemap).SaveToDirectory(_tempDir);

            Assert.True(ok);

            var files = Directory.GetFiles(_tempDir, "*.xml");
            Assert.NotEmpty(files);
            Assert.Contains(files, f => File.ReadAllText(f).Contains("example.com/page4"));
        }
    }
}

