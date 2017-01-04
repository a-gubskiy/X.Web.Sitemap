using System;
using System.Collections.Generic;
using System.IO;
using NSubstitute;
using NSubstituteAutoMocker;
using NUnit.Framework;
using Shouldly;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver
{
    [TestFixture]
    public class SerializeAndSaveTests
    {
        private NSubstituteAutoMocker<SerializedXmlSaver<SitemapIndex>> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<SerializedXmlSaver<SitemapIndex>>();
        }

        [Test]
        public void It_Throws_An_ArgumentNullException_If_There_Are_No_Sitemaps_Passed_In()
        {
            //--arrange

            //--act
            Assert.Throws<ArgumentNullException>(
                () => _autoMocker.ClassUnderTest.SerializeAndSave(null, new DirectoryInfo("c:\\temp"), "filename.xml"));
        }

        //--this is a half-assed test as comparing the full XML string that is generated is a big pain.
        [Test]
        public void It_Saves_The_XML_File_To_The_Correct_Directory_And_File_Name()
        {
            //--arrange
            var directory = new DirectoryInfo("x");
            string fileName = "sitemapindex.xml";

            var sitemapIndex = new SitemapIndex(new List<SitemapInfo>
            {
                new SitemapInfo(new Uri("http://example.com/sitemap1.xml"), DateTime.UtcNow),
                new SitemapInfo(new Uri("http://example.com/sitemap2.xml"), DateTime.UtcNow.AddDays(-1))
            });

            //--act
            _autoMocker.ClassUnderTest.SerializeAndSave(
                sitemapIndex,
                directory,
                fileName);

            //--assert
            _autoMocker.Get<IFileSystemWrapper>().Received().WriteFile(
                Arg.Is<string>(x => x.Contains("<sitemapindex")), 
                Arg.Is<DirectoryInfo>(x => x == directory), 
                Arg.Is<string>(x => x == fileName));
        }

        [Test]
        public void It_Returns_A_File_Info_For_The_File_That_Was_Created()
        {
            //--arrange
            var expectedFileInfo = new FileInfo("x");
            _autoMocker.Get<IFileSystemWrapper>().WriteFile(
                    Arg.Any<string>(),
                    Arg.Any<DirectoryInfo>(),
                    Arg.Any<string>())
                .Returns(expectedFileInfo);

            //--act
            var result = _autoMocker.ClassUnderTest.SerializeAndSave(
                new SitemapIndex(new List<SitemapInfo>()), 
                new DirectoryInfo("c:\\something\\"), 
                "file.xml");

            //--assert
            result.ShouldBeSameAs(expectedFileInfo);
        }

    }
}
