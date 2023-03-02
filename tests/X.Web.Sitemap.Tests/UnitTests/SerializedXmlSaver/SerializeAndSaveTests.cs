using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver;

[TestFixture]
public class SerializeAndSaveTests
{
	private IFileSystemWrapper _fileSystemWrapper;

	[SetUp]
	public void SetUp()
	{
		_fileSystemWrapper = new TestFileSystemWrapper();
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

		var serializer = new XmlSerializer(typeof(SitemapIndex));
		var path = Path.Combine(directory.FullName, fileName);
		var xml = "";
		
		using (var writer = new StringWriterUtf8())
		{
			serializer.Serialize(writer, sitemapIndex);
			xml= writer.ToString();
		}

		//--act
		var result = _fileSystemWrapper.WriteFile(xml, path);
		

		Assert.True(result.FullName.Contains("sitemapindex"));
		Assert.AreEqual(directory.Name, result.Directory.Name);
		Assert.AreEqual(fileName, result.Name);
	}

	[Test]
	public void It_Returns_A_File_Info_For_The_File_That_Was_Created()
	{
		//--arrange
		var expectedFileInfo = new FileInfo("something/file.xml");

		var sitemapIndex = new SitemapIndex(new List<SitemapInfo>());
		var directory = new DirectoryInfo("something");
		var fileName = "file.xml";
		var serializer = new XmlSerializer(typeof(SitemapIndex));
		var path = Path.Combine(directory.FullName, fileName);
		var xml = "";
		
		using (var writer = new StringWriterUtf8())
		{
			serializer.Serialize(writer, sitemapIndex);
			xml= writer.ToString();
		}

		//--act
		var result = _fileSystemWrapper.WriteFile(xml, path);

		Assert.AreEqual(expectedFileInfo.FullName, result.FullName);
		Assert.AreEqual(expectedFileInfo.Directory, result.Directory);
	}

}