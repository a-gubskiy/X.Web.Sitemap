using Xunit;
using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap.Tests.UnitTests;

public class SitemapExtensionInjectedTests : IDisposable
{
    private readonly string _tempDir;

    public SitemapExtensionInjectedTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private class FakeFsWrapper : IFileSystemWrapper
    {
        private readonly bool _createFile;

        public FakeFsWrapper(bool createFile)
        {
            _createFile = createFile;
        }

        public FileInfo WriteFile(string xml, string path)
        {
            if (_createFile)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
                File.WriteAllText(path, xml);
            }

            return new FileInfo(path);
        }

        public Task<FileInfo> WriteFileAsync(string xml, string path)
        {
            if (_createFile)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
                File.WriteAllText(path, xml);
            }

            return Task.FromResult(new FileInfo(path));
        }
    }

    [Fact]
    public void Save_WithInjectedWrapper_ReturnsTrueWhenFileCreated()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/inj1") };
        var path = Path.Combine(_tempDir, "inj1.xml");

        var wrapper = new FakeFsWrapper(true);

        // call internal overload explicitly
        var result = SitemapExtension.Save(sitemap, path, wrapper);

        Assert.True(result);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void Save_WithInjectedWrapper_ReturnsFalseWhenNotCreated()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/inj2") };
        var path = Path.Combine(_tempDir, "inj2.xml");

        var wrapper = new FakeFsWrapper(false);

        var result = SitemapExtension.Save(sitemap, path, wrapper);

        Assert.False(result);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task SaveAsync_WithInjectedWrapper_ReturnsTrueWhenFileCreated()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/inj3") };
        var path = Path.Combine(_tempDir, "inj3.xml");

        var wrapper = new FakeFsWrapper(true);

        var result = await SitemapExtension.SaveAsync(sitemap, path, wrapper);

        Assert.True(result);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task SaveAsync_WithInjectedWrapper_ReturnsFalseWhenNotCreated()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/inj4") };
        var path = Path.Combine(_tempDir, "inj4.xml");

        var wrapper = new FakeFsWrapper(false);

        var result = await SitemapExtension.SaveAsync(sitemap, path, wrapper);

        Assert.False(result);
        Assert.False(File.Exists(path));
    }
}