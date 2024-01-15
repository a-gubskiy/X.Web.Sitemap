using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver;

public class DeserializeTests
{
    [Fact]
    public void Check_That_XmlFile_Deserialized()
    {
        var xml = File.ReadAllText("Data/example.xml");
        var sitemap = Sitemap.Parse(xml);

        Assert.NotNull(sitemap);
    }
}