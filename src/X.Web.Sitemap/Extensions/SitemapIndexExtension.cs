using System.IO;
using JetBrains.Annotations;

namespace X.Web.Sitemap.Extensions;

/// <summary>
/// Provides extension methods for SitemapIndex.
/// </summary>
[PublicAPI]
public static class SitemapIndexExtension
{
    /// <summary>
    /// Converts a SitemapIndex to its XML string representation.
    /// </summary>
    /// <param name="sitemapIndex">The SitemapIndex object.</param>
    /// <returns>The XML string.</returns>
    public static string ToXml(this SitemapIndex sitemapIndex)
    {
        var serializer = new SitemapIndexSerializer();
        
        return serializer.Serialize(sitemapIndex);
    }
    
    /// <summary>
    /// Converts a SitemapIndex to a Stream.
    /// </summary>
    /// <param name="sitemapIndex">The SitemapIndex object.</param>
    /// <returns>The Stream containing the XML.</returns>
    public static Stream ToStream(this SitemapIndex sitemapIndex)
    {
        var xml = ToXml(sitemapIndex);
        var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        var stream = new MemoryStream(bytes);
       
        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;
    }
}