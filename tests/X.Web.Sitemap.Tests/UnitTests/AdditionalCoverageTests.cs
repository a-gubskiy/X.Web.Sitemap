using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests;

public class AdditionalCoverageTests : IDisposable
{
    private readonly string _tempDir;

    public AdditionalCoverageTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        // do not create directory to test CreateDirectory branch
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true); } catch { }
    }

    [Fact]
    public void Ctor_WithArray_AddsItems()
    {
        var arr = new Url[] { Url.CreateUrl("http://example.com/x") };
        var sitemap = new Sitemap(arr);
        Assert.Single(sitemap);
        Assert.Equal("http://example.com/x", sitemap[0].Location);
    }

    [Fact]
    public void TryParse_NullXml_ReturnsFalse_Sitemap()
    {
        var ok = Sitemap.TryParse(null!, out var sitemap);
        Assert.False(ok);
        Assert.Null(sitemap);
    }

    [Fact]
    public void TryParse_NullXml_ReturnsFalse_SitemapIndex()
    {
        var ok = SitemapIndex.TryParse(null!, out var sitemapIndex);
        Assert.False(ok);
        Assert.Null(sitemapIndex);
    }

    [Fact]
    public void SitemapIndexGenerator_StringOverload_WithInjectedWrapper_WritesFile()
    {
        var fileName = "sidx.xml";
        var info = new SitemapInfo(new Uri("http://test/s1.xml"), DateTime.UtcNow);

        var fake = new SitemapIndexGenerator(new TestFsWrapper(true, _tempDir));

        var index = fake.GenerateSitemapIndex(new[] { info }, _tempDir, fileName);

        Assert.NotNull(index);
        var path = Path.Combine(_tempDir, fileName);
        Assert.True(File.Exists(path));
        Assert.Contains("test/s1.xml", File.ReadAllText(path));
    }

    [Fact]
    public void FileSystemWrapper_WriteFile_CreatesNestedDirectory()
    {
        var wrapper = new FileSystemWrapper();
        var nested = Path.Combine(_tempDir, "a", "b", "c");
        var path = Path.Combine(nested, "nested.xml");
        var xml = "<x/>";

        var fi = wrapper.WriteFile(xml, path);

        Assert.True(fi.Exists);
        Assert.True(Directory.Exists(nested));
    }

    private class TestFsWrapper : IFileSystemWrapper
    {
        private readonly bool _create;
        private readonly string _base;
        public TestFsWrapper(bool create, string @base)
        {
            _create = create;
            _base = @base;
        }

        public FileInfo WriteFile(string xml, string path)
        {
            var full = Path.Combine(_base, Path.GetFileName(path));
            if (_create)
            {
                Directory.CreateDirectory(_base);
                File.WriteAllText(full, xml);
            }
            return new FileInfo(full);
        }

        public System.Threading.Tasks.Task<FileInfo> WriteFileAsync(string xml, string path)
        {
            return System.Threading.Tasks.Task.FromResult(WriteFile(xml, path));
        }
    }
}