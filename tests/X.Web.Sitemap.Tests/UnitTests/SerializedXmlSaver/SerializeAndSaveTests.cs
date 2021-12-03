using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver
{
	[TestFixture]
	public class SerializeAndSaveTests
	{
		private SerializedXmlSaver<SitemapIndex> _serializer;
		private IFileSystemWrapper _fileSystemWrapper;

		[SetUp]
		public void SetUp()
		{
			_fileSystemWrapper = new TestFileSystemWrapper();
			_serializer = new SerializedXmlSaver<SitemapIndex>(_fileSystemWrapper);
		}

		[Test]
		public void It_Throws_An_ArgumentNullException_If_There_Are_No_Sitemaps_Passed_In()
		{
			Assert.Throws<ArgumentNullException>(
				() => _serializer.SerializeAndSave(null, new DirectoryInfo(Path.GetTempPath()), "filename.xml"));
		}

		//--this is a half-assed test as comparing the full XML string that is generated is a big pain.
		[Test]
		public void It_Saves_The_XML_File_To_The_Correct_Directory_And_File_Name()
		{
			//--arrange
			var directory = new DirectoryInfo("x");
			var fileName = "sitemapindex.xml";

			var sitemapIndex = new SitemapIndex(new List<SitemapInfo>
			{
				new SitemapInfo(new Uri("http://example.com/sitemap1.xml"), DateTime.UtcNow),
				new SitemapInfo(new Uri("http://example.com/sitemap2.xml"), DateTime.UtcNow.AddDays(-1))
			});

			//--act
			var result = _serializer.SerializeAndSave(
				 sitemapIndex,
				 directory,
				 fileName);

			Assert.True(result.FullName.Contains("sitemapindex"));
			Assert.AreEqual(directory.Name, result.Directory.Name);
			Assert.AreEqual(fileName, result.Name);
		}

		[Test]
		public void It_Returns_A_File_Info_For_The_File_That_Was_Created()
		{
			//--arrange
			var expectedFileInfo = new FileInfo("something/file.xml");

			//--act
			var result = _serializer.SerializeAndSave(
				new SitemapIndex(new List<SitemapInfo>()),
				new DirectoryInfo("something"),
				"file.xml");


			Assert.AreEqual(expectedFileInfo.FullName, result.FullName);
			Assert.AreEqual(expectedFileInfo.Directory, result.Directory);
		}

	}
}
