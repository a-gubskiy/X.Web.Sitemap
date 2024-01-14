using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests.SitemapIndexGeneratorTests;

public class GenerateSitemapIndexTests
{
    private SitemapIndexGenerator _sitemapIndexGenerator;
    private TestFileSystemWrapper _fileSystemWrapper;

    public GenerateSitemapIndexTests()
    {
        _fileSystemWrapper = new TestFileSystemWrapper();
        _sitemapIndexGenerator = new SitemapIndexGenerator(_fileSystemWrapper);
    }

    [Fact]
    public void It_Saves_A_Generated_Sitemap_Index_File_From_The_Specified_Sitemaps()
    {
        //--arrange
        var sitemaps = new List<SitemapInfo>
        {
            new SitemapInfo(new Uri("https://example.com"), DateTime.UtcNow),
            new SitemapInfo(new Uri("https://example2.com"), DateTime.UtcNow.AddDays(-1))
        };

        var expectedDirectory = new DirectoryInfo(@"C:\temp\sitemaptests\");
        var expectedFilename = "testSitemapIndex1.xml"; //--act
        var sitemapIndex =
            _sitemapIndexGenerator.GenerateSitemapIndex(sitemaps, expectedDirectory, expectedFilename);

        // Custom assertion
        Assert.True(AssertCorrectSitemapIndexWasSerialized(sitemaps, sitemapIndex));
    }

    private bool AssertCorrectSitemapIndexWasSerialized(IEnumerable<SitemapInfo> expectedSitemaps,
        SitemapIndex actualSitemapIndex)
    {
        foreach (var expectedSitemap in expectedSitemaps)
        {
            if (!actualSitemapIndex.Sitemaps.Contains(expectedSitemap))
            {
                // xUnit does not have a direct equivalent of Assert.Fail, so we throw an exception instead
                throw new Xunit.Sdk.XunitException(
                    "Received a call to .SerializeAndSave, but at least one of the expected sitemapInfos was missing.");
            }
        }

        return true;
    }
}