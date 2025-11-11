using X.Web.Sitemap.Serializers;
using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests;

public class SitemapSerializerTests
{
    [Fact]
    public void Serialize_Null_ThrowsArgumentNullException()
    {
        var serializer = new SitemapSerializer();
        Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null!));
    }

    [Fact]
    public void Deserialize_Empty_ThrowsArgumentException()
    {
        var serializer = new SitemapSerializer();
        Assert.Throws<ArgumentNullException>(() => serializer.Deserialize(string.Empty));
    }

    [Fact]
    public void Deserialize_InvalidXml_ThrowsInvalidOperationException()
    {
        var serializer = new SitemapSerializer();
        Assert.Throws<InvalidOperationException>(() => serializer.Deserialize("<notvalid></notvalid>"));
    }

    [Fact]
    public void SerializeAndDeserialize_RoundTrip_Works()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/rt") };
        var serializer = new SitemapSerializer();

        var xml = serializer.Serialize(sitemap);
        Assert.False(string.IsNullOrWhiteSpace(xml));

        var deserialized = serializer.Deserialize(xml);
        Assert.NotNull(deserialized);
        Assert.Single(deserialized);
        Assert.Equal("http://example.com/rt", deserialized[0].Location);
    }

    [Fact]
    public void Serialize_RootHasDefaultSitemapNamespace()
    {
        var sitemap = new Sitemap { Url.CreateUrl("http://example.com/") };
        var serializer = new SitemapSerializer();

        var xml = serializer.Serialize(sitemap);

        // The root should start with the urlset element and default sitemap namespace
        Assert.Contains("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"", xml);
    }
}