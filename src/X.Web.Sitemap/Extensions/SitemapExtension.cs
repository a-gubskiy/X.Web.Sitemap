using System.IO;
using System.Xml;
using JetBrains.Annotations;

namespace X.Web.Sitemap.Extensions;

[PublicAPI]
public static class SitemapExtension
{
    public static string ToXml(this ISitemap sitemap)
    {
        var serializer = new SitemapSerializer();
        
        return serializer.Serialize(sitemap);
    }
    
    public static Stream ToStream(this ISitemap sitemap)
    {
        var serializer = new SitemapSerializer();
        var xml = serializer.Serialize(sitemap);
        var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        var stream = new MemoryStream(bytes);
        
        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;
    }
}