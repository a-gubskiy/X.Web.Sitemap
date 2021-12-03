using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace X.Web.Sitemap.Tests.UnitTests.SitemapIndexGeneratorTests;

[TestFixture]
public class GenerateSitemapIndexTests
{
	private SitemapIndexGenerator _sitemapIndexGenerator;
	private ISerializedXmlSaver<SitemapIndex> _sitemapIndexSerializer;
	private TestFileSystemWrapper _fileSystemWrapper;

	[SetUp]
	public void SetUp()
	{
		_fileSystemWrapper = new TestFileSystemWrapper();
		_sitemapIndexSerializer = new SerializedXmlSaver<SitemapIndex>(_fileSystemWrapper);
		_sitemapIndexGenerator = new SitemapIndexGenerator(_sitemapIndexSerializer);
	}

	[Test]
	public void It_Saves_A_Generated_Sitemap_Index_File_From_The_Specified_Sitemaps()
	{
		//--arrange
		var sitemaps = new List<SitemapInfo>
		{
			new SitemapInfo(new Uri("https://example.com"), DateTime.UtcNow),
			new SitemapInfo(new Uri("https://example2.com"), DateTime.UtcNow.AddDays(-1))
		};
		var expectedDirectory = new DirectoryInfo(@"C:\temp\sitemaptests\");
		var expectedFilename = "testSitemapIndex1.xml";

		//--act
		var sitemapIndex = _sitemapIndexGenerator.GenerateSitemapIndex(sitemaps, expectedDirectory, expectedFilename);
		Assert.True(AssertCorrectSitemapIndexWasSerialized(sitemaps, sitemapIndex));

		//--assert
		//_sitemapIndexSerializer
		//	.Received()
		//	.SerializeAndSave(
		//		Arg.Is<SitemapIndex>(x => AssertCorrectSitemapIndexWasSerialized(sitemaps, x)),
		//		Arg.Is<DirectoryInfo>(x => x == expectedDirectory),
		//		Arg.Is<string>(x => x == expectedFilename));
	}

	private bool AssertCorrectSitemapIndexWasSerialized(IEnumerable<SitemapInfo> expectedSitemaps, SitemapIndex actualSitemapIndex)
	{
		foreach (var expectedSitemap in expectedSitemaps)
		{
			if (!actualSitemapIndex.Sitemaps.Contains(expectedSitemap))
			{
				Assert.Fail("Received a call to .SerializeAndSave, but at least one of the expected sitemapInfos was missing.");
			}
		}

		return true;
	}
}