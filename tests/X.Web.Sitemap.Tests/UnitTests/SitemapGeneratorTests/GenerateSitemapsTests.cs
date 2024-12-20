using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests.SitemapGeneratorTests;

public class GenerateSitemapsTests
{
    private SitemapGenerator _sitemapGenerator;

    public GenerateSitemapsTests()
    {
        _sitemapGenerator = new SitemapGenerator();
    }

    [Fact]
    public void It_Only_Saves_One_Sitemap_If_There_Are_Less_Than_50001_Urls()
    {
        var filesCount = 4;
        var recordsCount = (Sitemap.DefaultMaxNumberOfUrlsPerSitemap * 3) + 5;
        var urls = new List<Url>();

        for (var i = 0; i < recordsCount; i++)
        {
            urls.Add(new Url());
        }

        var result = _sitemapGenerator.GenerateSitemaps(urls, new DirectoryInfo("x"), "file");

        Assert.Equal(filesCount, result.Count);
    }

    [Fact]
    public void It_Saves_Two_Sitemaps_If_There_Are_More_Than_50000_Urls_But_Less_Than_100001_And_It_Names_The_Files_With_A_Three_Digit_Suffix_Incrementing_For_Each_One()
    {
        //--arrange
        var enoughForTwoSitemaps = Sitemap.DefaultMaxNumberOfUrlsPerSitemap + 1;
        var urls = new List<Url>(enoughForTwoSitemaps);
        var filesCount = 2;

        for (var i = 0; i < enoughForTwoSitemaps; i++)
        {
            urls.Add(new Url());
        }

        var fileName = "file";
        var directory = new DirectoryInfo("x");

        //--act
        var result = _sitemapGenerator.GenerateSitemaps(urls, directory, fileName);

        Assert.Equal(filesCount, result.Count);
        Assert.All(result, o => Assert.Equal(directory.Name, o.Directory?.Name));
        Assert.Contains(result, o => o.Name == "file-1.xml");
        Assert.Contains(result, o => o.Name == "file-2.xml");
    }
}