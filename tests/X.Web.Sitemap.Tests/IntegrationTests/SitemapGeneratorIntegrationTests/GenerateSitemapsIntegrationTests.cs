using Xunit;

namespace X.Web.Sitemap.Tests.IntegrationTests.SitemapGeneratorIntegrationTests;

public class GenerateSitemapsIntegrationTests : IDisposable
{
    private SitemapGenerator _sitemapGenerator;
    private readonly string _sitemapLocation = Path.GetTempPath();

    public GenerateSitemapsIntegrationTests()
    {
        _sitemapGenerator = new SitemapGenerator();
    }

    public void Dispose()
    {
        // Cleanup code if needed
    }

    [Fact]
    public void It_Only_Saves_One_Sitemap_If_There_Are_Less_Than_50001_Urls()
    {
        //--arrange
        var maxNumberOfUrlsForOneSitemap = Sitemap.DefaultMaxNumberOfUrlsPerSitemap;
        var urls = new List<Url>(maxNumberOfUrlsForOneSitemap);
        var now = DateTime.UtcNow;
            
        for (var i = 0; i < maxNumberOfUrlsForOneSitemap; i++)
        {
            urls.Add(Url.CreateUrl("https://example.com/" + i, now));
        }

        //--act
        _sitemapGenerator.GenerateSitemaps(urls, new DirectoryInfo(_sitemapLocation), "sitemap_from_test_1");

        //--assert
        //--go look in the {sitemapLocation} directory!
    }

    [Fact]
    public void It_Saves_Two_Sitemaps_If_There_Are_More_Than_50000_Urls_But_Less_Than_100001_And_It_Names_The_Files_With_A_Three_Digit_Suffix_Incrementing_For_Each_One()
    {
        //--arrange
        var enoughUrlsForTwoSitemaps = Sitemap.DefaultMaxNumberOfUrlsPerSitemap + 1;
        var urls = new List<Url>(enoughUrlsForTwoSitemaps);
        var now = DateTime.UtcNow;
            
        for (var i = 0; i < enoughUrlsForTwoSitemaps; i++)
        {
            urls.Add(Url.CreateUrl("https://example.com/" + i, now));
        }

        //--act
        _sitemapGenerator.GenerateSitemaps(urls, new DirectoryInfo(_sitemapLocation), "sitemap_from_test_2");

        //--assert
        //--go look for 2 sitemaps in the {sitemapLocation} directory!
    }
}