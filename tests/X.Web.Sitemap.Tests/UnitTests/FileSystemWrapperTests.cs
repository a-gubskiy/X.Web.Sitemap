using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class FileSystemWrapperTests : IDisposable
    {
        private readonly string _tempDir;

        public FileSystemWrapperTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempDir))
                {
                    Directory.Delete(_tempDir, true);
                }
            }
            catch
            {
                // best-effort cleanup
            }
        }

        [Fact]
        public void WriteFile_CreatesFileAndReturnsFileInfo()
        {
            var wrapper = new FileSystemWrapper();
            var path = Path.Combine(_tempDir, "sitemap.xml");
            var xml = "<root>hello</root>";

            var fi = wrapper.WriteFile(xml, path);

            Assert.True(fi.Exists);
            Assert.Equal(path, fi.FullName);
            Assert.Equal(xml, File.ReadAllText(path));
        }

        [Fact]
        public async Task WriteFileAsync_CreatesFileAndReturnsFileInfo()
        {
            var wrapper = new FileSystemWrapper();
            var path = Path.Combine(_tempDir, "async-sitemap.xml");
            var xml = "<root>async</root>";

            var fi = await wrapper.WriteFileAsync(xml, path);

            Assert.True(fi.Exists);
            Assert.Equal(path, fi.FullName);
            Assert.Equal(xml, File.ReadAllText(path));
        }

        [Fact]
        public void WriteFile_NoDirectory_ThrowsArgumentException()
        {
            var wrapper = new FileSystemWrapper();
            // Path without directory part
            var path = "just-a-file.xml";

            Assert.Throws<ArgumentException>(() => wrapper.WriteFile("x", path));
        }

        [Fact]
        public async Task WriteFileAsync_NoDirectory_ThrowsArgumentException()
        {
            var wrapper = new FileSystemWrapper();
            // Path without directory part
            var path = "another-file.xml";

            await Assert.ThrowsAsync<ArgumentException>(async () => await wrapper.WriteFileAsync("x", path));
        }
    }
}

