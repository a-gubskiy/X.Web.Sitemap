using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver
{
	[TestFixture]
	public class DeserializeTests
	{
		[SetUp]
		public void SetUp()
		{
			
		}

		[Test]
		public void Check_That_XmlFile_Deserialized()
		{
			//--arrange

			var xml = File.ReadAllText("Data/example.xml");;
			var sitemap = Sitemap.Parse(xml);
			
			//--act
			Assert.NotNull(sitemap);
		}
	}
}
