using X.Web.Sitemap.Serializers;
using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver;

public class SerializeAndSaveTests
{
    private readonly IFileSystemWrapper _fileSystemWrapper;

    public SerializeAndSaveTests()
    {
        _fileSystemWrapper = new TestFileSystemWrapper();
    }

    [Fact]
    public void It_Saves_The_XML_File_To_The_Correct_Directory_And_File_Name()
    {
        //--arrange
        var sitemapIndex = new SitemapIndex(new List<SitemapInfo>
        {
            new SitemapInfo(new Uri("http://example.com/sitemap1.xml"), DateTime.UtcNow),
            new SitemapInfo(new Uri("http://example.com/sitemap2.xml"), DateTime.UtcNow.AddDays(-1))
        });

        var fileName = "sitemapindex.xml";
        var directory = new DirectoryInfo("x");
        var path = Path.Combine(directory.FullName, fileName);

        var serializer = new SitemapIndexSerializer();
        var xml = serializer.Serialize(sitemapIndex);

        //--act
        var result = _fileSystemWrapper.WriteFile(xml, path);

        //--assert
        Assert.Contains("sitemapindex", result.FullName);
        
        Assert.Equal(directory.Name, result.Directory?.Name);
        Assert.Equal(fileName, result.Name);
    }

    [Fact]
    public void It_Returns_A_File_Info_For_The_File_That_Was_Created()
    {
        //--arrange
        var expectedFileInfo = new FileInfo("something/file.xml");
        var sitemapIndex = new SitemapIndex(new List<SitemapInfo>());

        var serializer = new SitemapIndexSerializer();
        var xml = serializer.Serialize(sitemapIndex);

        var fileName = "file.xml";
        var directory = new DirectoryInfo("something");
        var path = Path.Combine(directory.FullName, fileName);

        //--act
        var result = _fileSystemWrapper.WriteFile(xml, path);

        //--assert
        Assert.Equal(expectedFileInfo.FullName, result.FullName);
        Assert.Equal(expectedFileInfo.Directory?.Name, result.Directory?.Name);
    }
}