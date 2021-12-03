using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver
{
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
		public void Check_That_XmlFile_HashSitemap_Deserialized()
		{
			var xml = File.ReadAllText("Data/example.xml");
			var sitemap = HashSitemap.Parse(xml);
			
			Assert.NotNull(sitemap);
		}
	}
}
