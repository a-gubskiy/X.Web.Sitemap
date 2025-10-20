using System.Xml.Serialization;
using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests;

public class ImageTests
{
    [Fact]
    public void Defaults_AreCorrect()
    {
        var img = new Image();
        Assert.Equal(string.Empty, img.Location);
        Assert.Null(img.Caption);
        Assert.Null(img.GeographicLocation);
        Assert.Null(img.Title);
        Assert.Null(img.License);
    }

    [Fact]
    public void Serialization_IncludesLoc()
    {
        var img = new Image { Location = "http://example.com/image.png", Title = "T" };
        var serializer = new XmlSerializer(typeof(Image));

        using var writer = new StringWriter();
        serializer.Serialize(writer, img);
        var xml = writer.ToString();

        Assert.Contains("http://example.com/image.png", xml);
        Assert.Contains("loc", xml);
    }
}