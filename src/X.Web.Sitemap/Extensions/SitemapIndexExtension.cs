using System.IO;
using JetBrains.Annotations;

namespace X.Web.Sitemap.Extensions;

[PublicAPI]
public static class SitemapIndexExtension
{
    public static string ToXml(this SitemapIndex sitemapIndex)
    {
        var serializer = new SitemapIndexSerializer();
        
        return serializer.Serialize(sitemapIndex);
    }
    
    public static Stream ToStream(this SitemapIndex sitemapIndex)
    {
        var serializer = new SitemapIndexSerializer();
        var xml = serializer.Serialize(sitemapIndex);
        var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        var stream = new MemoryStream(bytes);
       
        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;
    }
}