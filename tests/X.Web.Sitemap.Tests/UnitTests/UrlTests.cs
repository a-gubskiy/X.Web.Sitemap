using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests;

public class UrlTests
{
    [Fact]
    public void CreateUrl_SetsProperties()
    {
        var now = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc).ToLocalTime();
        var url = Url.CreateUrl("http://example.com", now);

        Assert.Equal("http://example.com", url.Location);
        Assert.Equal(0.5d, url.Priority);
        Assert.Equal(now, url.TimeStamp);
        Assert.Null(url.ChangeFrequency);
        Assert.NotNull(url.Images);
    }

    [Fact]
    public void LastMod_ParsesIsoWithOffset_ToUtcEquivalent()
    {
        var url = new Url();
        url.LastMod = "2004-12-23T18:00:15+00:00";

        var expectedUtc = new DateTime(2004, 12, 23, 18, 0, 15, DateTimeKind.Utc);
        Assert.NotNull(url.TimeStamp);
        Assert.Equal(expectedUtc, url.TimeStamp.Value.ToUniversalTime());
    }

    [Fact]
    public void LastMod_EmptyString_SetsNullTimeStamp()
    {
        var url = new Url();
        url.LastMod = string.Empty;

        Assert.Null(url.TimeStamp);
    }
}