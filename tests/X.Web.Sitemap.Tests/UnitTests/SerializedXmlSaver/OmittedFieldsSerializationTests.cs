using X.Web.Sitemap.Serializers;
using Xunit;

namespace X.Web.Sitemap.Tests.UnitTests.SerializedXmlSaver;

public class OmittedFieldsSerializationTests
{
    [Fact]
    public void Url_Without_ChangeFreq_Should_Not_Include_ChangeFreq_Element()
    {
        // Arrange
        var url = new Url
        {
            Location = "https://example.com/page1",
            Priority = 0.8d,
            ChangeFrequency = null // Explicitly null
        };

        var sitemap = new Sitemap();
        sitemap.Add(url);

        var serializer = new SitemapSerializer();

        // Act
        var xml = serializer.Serialize(sitemap);

        // Assert
        Assert.DoesNotContain("<changefreq>", xml);
        Assert.Contains("<loc>https://example.com/page1</loc>", xml);
        Assert.Contains("<priority>0.8</priority>", xml);
    }

    [Fact]
    public void Url_Without_LastMod_Should_Not_Include_LastMod_Element()
    {
        // Arrange
        var url = new Url
        {
            Location = "https://example.com/page2",
            Priority = 0.5d,
            TimeStamp = null // Explicitly null
        };

        var sitemap = new Sitemap();
        sitemap.Add(url);

        var serializer = new SitemapSerializer();

        // Act
        var xml = serializer.Serialize(sitemap);

        // Assert
        Assert.DoesNotContain("<lastmod>", xml);
        Assert.Contains("<loc>https://example.com/page2</loc>", xml);
        Assert.Contains("<priority>0.5</priority>", xml);
    }

    [Fact]
    public void Url_Without_Both_ChangeFreq_And_LastMod_Should_Only_Include_Loc_And_Priority()
    {
        // Arrange
        var url = new Url
        {
            Location = "https://example.com/page3",
            Priority = 0.3d,
            TimeStamp = null,
            ChangeFrequency = null
        };

        var sitemap = new Sitemap();
        sitemap.Add(url);

        var serializer = new SitemapSerializer();

        // Act
        var xml = serializer.Serialize(sitemap);

        // Assert
        Assert.DoesNotContain("<lastmod>", xml);
        Assert.DoesNotContain("<changefreq>", xml);
        Assert.Contains("<loc>https://example.com/page3</loc>", xml);
        Assert.Contains("<priority>0.3</priority>", xml);
    }

    [Fact]
    public void Url_With_ChangeFreq_Should_Include_ChangeFreq_Element()
    {
        // Arrange
        var url = new Url
        {
            Location = "https://example.com/page4",
            ChangeFrequency = ChangeFrequency.Weekly
        };

        var sitemap = new Sitemap();
        sitemap.Add(url);

        var serializer = new SitemapSerializer();

        // Act
        var xml = serializer.Serialize(sitemap);

        // Assert
        Assert.Contains("<changefreq>weekly</changefreq>", xml);
        Assert.Contains("<loc>https://example.com/page4</loc>", xml);
    }

    [Fact]
    public void Url_With_LastMod_Should_Include_LastMod_Element()
    {
        // Arrange
        var url = new Url
        {
            Location = "https://example.com/page5",
            TimeStamp = new DateTime(2023, 5, 15, 10, 30, 0, DateTimeKind.Utc)
        };

        var sitemap = new Sitemap();
        sitemap.Add(url);

        var serializer = new SitemapSerializer();

        // Act
        var xml = serializer.Serialize(sitemap);

        // Assert
        Assert.Contains("<lastmod>", xml);
        Assert.Contains("<loc>https://example.com/page5</loc>", xml);
    }

    [Fact]
    public void Multiple_Urls_With_Mixed_Optional_Fields_Should_Serialize_Correctly()
    {
        // Arrange
        var url1 = new Url
        {
            Location = "https://example.com/with-all",
            TimeStamp = DateTime.UtcNow,
            ChangeFrequency = ChangeFrequency.Daily,
            Priority = 1.0d
        };

        var url2 = new Url
        {
            Location = "https://example.com/without-changefreq",
            TimeStamp = DateTime.UtcNow,
            ChangeFrequency = null,
            Priority = 0.8d
        };

        var url3 = new Url
        {
            Location = "https://example.com/without-lastmod",
            TimeStamp = null,
            ChangeFrequency = ChangeFrequency.Monthly,
            Priority = 0.6d
        };

        var url4 = new Url
        {
            Location = "https://example.com/minimal",
            TimeStamp = null,
            ChangeFrequency = null,
            Priority = 0.5d
        };

        var sitemap = new Sitemap();
        sitemap.Add(url1);
        sitemap.Add(url2);
        sitemap.Add(url3);
        sitemap.Add(url4);

        var serializer = new SitemapSerializer();

        // Act
        var xml = serializer.Serialize(sitemap);

        // Assert
        // URL 1 - should have all fields
        Assert.Contains("<loc>https://example.com/with-all</loc>", xml);
        var url1Start = xml.IndexOf("<loc>https://example.com/with-all</loc>");
        var url2Start = xml.IndexOf("<loc>https://example.com/without-changefreq</loc>");
        var url1Section = xml.Substring(url1Start, url2Start - url1Start);
        Assert.Contains("<lastmod>", url1Section);
        Assert.Contains("<changefreq>daily</changefreq>", url1Section);

        // URL 2 - should have lastmod but not changefreq
        var url3Start = xml.IndexOf("<loc>https://example.com/without-lastmod</loc>");
        var url2Section = xml.Substring(url2Start, url3Start - url2Start);
        Assert.Contains("<lastmod>", url2Section);
        Assert.DoesNotContain("<changefreq>", url2Section);

        // URL 3 - should have changefreq but not lastmod
        var url4Start = xml.IndexOf("<loc>https://example.com/minimal</loc>");
        var url3Section = xml.Substring(url3Start, url4Start - url3Start);
        Assert.DoesNotContain("<lastmod>", url3Section);
        Assert.Contains("<changefreq>monthly</changefreq>", url3Section);

        // URL 4 - should have neither
        var url4End = xml.IndexOf("</urlset>");
        var url4Section = xml.Substring(url4Start, url4End - url4Start);
        Assert.DoesNotContain("<lastmod>", url4Section);
        Assert.DoesNotContain("<changefreq>", url4Section);
        Assert.Contains("<priority>0.5</priority>", url4Section);
    }

    [Fact]
    public void SitemapInfo_Without_LastMod_Should_Not_Include_LastMod_Element()
    {
        // Arrange
        var sitemapInfo = new SitemapInfo(new Uri("https://example.com/sitemap1.xml"));

        var sitemapIndex = new SitemapIndex(new List<SitemapInfo> { sitemapInfo });

        var serializer = new SitemapIndexSerializer();

        // Act
        var xml = serializer.Serialize(sitemapIndex);

        // Assert
        Assert.DoesNotContain("<lastmod>", xml);
        Assert.Contains("<loc>https://example.com/sitemap1.xml</loc>", xml);
    }

    [Fact]
    public void SitemapInfo_With_LastMod_Should_Include_LastMod_Element()
    {
        // Arrange
        var lastMod = new DateTime(2023, 6, 20, 14, 30, 0, DateTimeKind.Utc);
        var sitemapInfo = new SitemapInfo(new Uri("https://example.com/sitemap2.xml"), lastMod);

        var sitemapIndex = new SitemapIndex(new List<SitemapInfo> { sitemapInfo });

        var serializer = new SitemapIndexSerializer();

        // Act
        var xml = serializer.Serialize(sitemapIndex);

        // Assert
        Assert.Contains("<lastmod>", xml);
        Assert.Contains("<loc>https://example.com/sitemap2.xml</loc>", xml);
    }

    [Fact]
    public void Multiple_SitemapInfos_With_Mixed_LastMod_Should_Serialize_Correctly()
    {
        // Arrange
        var sitemapInfo1 = new SitemapInfo(
            new Uri("https://example.com/sitemap-with-lastmod.xml"), 
            DateTime.UtcNow
        );

        var sitemapInfo2 = new SitemapInfo(
            new Uri("https://example.com/sitemap-without-lastmod.xml")
        );

        var sitemapIndex = new SitemapIndex(new List<SitemapInfo> { sitemapInfo1, sitemapInfo2 });

        var serializer = new SitemapIndexSerializer();

        // Act
        var xml = serializer.Serialize(sitemapIndex);

        // Assert
        // First sitemap should have lastmod
        var sitemap1Start = xml.IndexOf("<loc>https://example.com/sitemap-with-lastmod.xml</loc>");
        var sitemap2Start = xml.IndexOf("<loc>https://example.com/sitemap-without-lastmod.xml</loc>");
        var sitemap1Section = xml.Substring(sitemap1Start, sitemap2Start - sitemap1Start);
        Assert.Contains("<lastmod>", sitemap1Section);

        // Second sitemap should not have lastmod
        var sitemapIndexEnd = xml.IndexOf("</sitemapindex>");
        var sitemap2Section = xml.Substring(sitemap2Start, sitemapIndexEnd - sitemap2Start);
        Assert.DoesNotContain("<lastmod>", sitemap2Section);
    }
}
