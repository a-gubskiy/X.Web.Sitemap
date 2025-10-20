using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests;

public class SitemapCtorAndTryParseTests
{
    [Fact]
    public void Ctor_WithEnumerable_AddsItems()
    {
        var urls = new List<Url>
        {
            Url.CreateUrl("http://example.com/a"),
            Url.CreateUrl("http://example.com/b")
        };

        var sitemap = new Sitemap(urls);

        Assert.Equal(2, sitemap.Count);
        Assert.Contains(sitemap, u => u.Location == "http://example.com/a");
        Assert.Contains(sitemap, u => u.Location == "http://example.com/b");
    }

    [Fact]
    public void TryParse_ValidXml_ReturnsTrueAndSitemap()
    {
        var xml = "<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">" +
                  "<url><loc>http://test/</loc></url></urlset>";

        var ok = Sitemap.TryParse(xml, out var sitemap);

        Assert.True(ok);
        Assert.NotNull(sitemap);
        Assert.Single(sitemap);
        Assert.Equal("http://test/", sitemap[0].Location);
    }

    [Fact]
    public void TryParse_InvalidXml_ReturnsFalseAndNull()
    {
        var xml = "<notvalid></notvalid>";
        var ok = Sitemap.TryParse(xml, out var sitemap);

        Assert.False(ok);
        Assert.Null(sitemap);
    }
}