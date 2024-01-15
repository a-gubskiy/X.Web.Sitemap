using Xunit;

namespace X.Web.Sitemap.Tests.IntegrationTests.SitemapIndexGeneratorIntegrationTests;

public class GenerateSitemapIndexIntegrationTests : IDisposable
{
    private SitemapIndexGenerator _sitemapIndexGenerator;
    private readonly string _sitemapLocation = Path.GetTempPath();

    public GenerateSitemapIndexIntegrationTests()
    {
        _sitemapIndexGenerator = new SitemapIndexGenerator();
    }

    public void Dispose()
    {
        // Cleanup code if needed
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

        var expectedDirectory = new DirectoryInfo(_sitemapLocation);
        var expectedFilename = "testSitemapIndex1.xml";

        //--act
        _sitemapIndexGenerator.GenerateSitemapIndex(sitemaps, expectedDirectory, expectedFilename);

        //--assert
        //--go looks in the {sitemapLocation} directory
    }
}