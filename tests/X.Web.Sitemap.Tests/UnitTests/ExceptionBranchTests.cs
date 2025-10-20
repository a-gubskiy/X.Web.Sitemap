using Xunit;
using X.Web.Sitemap.Extensions;
using X.Web.Sitemap.Generators;

namespace X.Web.Sitemap.Tests.UnitTests;

public class ExceptionBranchTests : IDisposable
{
    private readonly string _tempDir;

    public ExceptionBranchTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private class ThrowingFsWrapper : IFileSystemWrapper
    {
        public FileInfo WriteFile(string xml, string path)
        {
            throw new InvalidOperationException("boom");
        }

        public Task<FileInfo> WriteFileAsync(string xml, string path)
        {
            throw new InvalidOperationException("boom async");
        }
    }

    [Fact]
    public void Save_InternalOverload_ReturnsFalseOnException()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/ex1") };
        var path = Path.Combine(_tempDir, "ex1.xml");

        var wrapper = new ThrowingFsWrapper();

        var result = SitemapExtension.Save(sitemap, path, wrapper);

        Assert.False(result);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task SaveAsync_InternalOverload_ReturnsFalseOnException()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/ex2") };
        var path = Path.Combine(_tempDir, "ex2.xml");

        var wrapper = new ThrowingFsWrapper();

        var result = await SitemapExtension.SaveAsync(sitemap, path, wrapper);

        Assert.False(result);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void SitemapIndexGenerator_StringOverload_Default_WritesFile()
    {
        var info = new SitemapInfo(new Uri("http://example.com/s1.xml"), DateTime.UtcNow);
        var generator = new SitemapIndexGenerator();
        var fileName = "sidx2.xml";

        // call string-overload directly
        var index = generator.GenerateSitemapIndex(new[] { info }, _tempDir, fileName);

        var path = Path.Combine(_tempDir, fileName);
        Assert.True(File.Exists(path));
        Assert.Contains("s1.xml", File.ReadAllText(path));
    }
}