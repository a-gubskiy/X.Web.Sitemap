using NUnit.Framework;
using System.IO;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver;

[TestFixture]
public class DeserializeTests
{
	[Test]
	public void Check_That_XmlFile_Deserialized()
	{
		var xml = File.ReadAllText("Data/example.xml");
		var sitemap = Sitemap.Parse(xml);
			
		Assert.NotNull(sitemap);
	}
		
		
	[Test]
	public void Check_That_Hreflang_Fields_Deserialized()
	{
		var xml = File.ReadAllText("Data/example.xml");
		var sitemap = Sitemap.Parse(xml);
			
		Assert.Null(sitemap[0].Language);
		Assert.Null(sitemap[1].Language);
		Assert.NotNull(sitemap[2].Language);
		Assert.NotNull(sitemap[3].Language);
		Assert.NotNull(sitemap[4].Language);
	}
}