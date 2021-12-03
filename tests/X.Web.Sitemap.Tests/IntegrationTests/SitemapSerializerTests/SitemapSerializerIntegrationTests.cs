using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace X.Web.Sitemap.Tests.IntegrationTests.SitemapSerializerTests;

[TestFixture]
public class SitemapSerializerIntegrationTests
{
    private ISitemapSerializer _sitemapSerializer;
    private readonly string _sitemapLocation = Path.GetTempPath();

    [SetUp]
    public void SetUp()
    {
        _sitemapSerializer = new SitemapSerializer
        {
            MaxNumberOfUrlsPerSitemap = 100
        };
    }

    [Test]
    public void Test_SaveToDirectory()
    {
        var sitemap = new HashSitemap();

        for (int i = 0; i < 1000; i++)
        {
            sitemap.Add(new Url
            {
                Location = $"https://example.com/{i}/",
                Priority = 1.0,
                ChangeFrequency = ChangeFrequency.Daily,
                LastMod = DateTime.Now.ToString("yyyy-MM-dd"),
                TimeStamp = DateTime.Now,
                Language = "en"
            });
        }

        var directory = Path.Combine(Path.GetTempPath(), "sitemap-test");
        
        _sitemapSerializer.SaveToDirectory(directory, sitemap);
        var files = Directory.GetFiles(directory);
        
        Assert.AreEqual(10, files.Length);
    }
}