using System.IO;
using System.Threading.Tasks;
using X.Web.Sitemap.Generators;
using X.Web.Sitemap.Serializers;

namespace X.Web.Sitemap.Extensions;

/// <summary>
/// Provides extension methods for ISitemap.
/// </summary>
public static class SitemapExtension
{
    /// <summary>
    /// Converts an ISitemap to its XML string representation.
    /// </summary>
    /// <param name="sitemap">The ISitemap object.</param>
    /// <returns>The XML string.</returns>
    public static string ToXml(this ISitemap sitemap)
    {
        var serializer = new SitemapSerializer();
        
        return serializer.Serialize(sitemap);
    }
    
    /// <summary>
    /// Converts an ISitemap to a Stream.
    /// </summary>
    /// <param name="sitemap">The ISitemap object.</param>
    /// <returns>The Stream containing the XML.</returns>
    public static Stream ToStream(this ISitemap sitemap)
    {
        var xml = ToXml(sitemap);
        var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
        var stream = new MemoryStream(bytes);
        
        stream.Seek(0, SeekOrigin.Begin);
        
        return stream;
    }

    /// <summary>
    /// Saves the ISitemap to a directory.
    /// </summary>
    /// <param name="sitemap">The ISitemap object.</param>
    /// <param name="targetSitemapDirectory">The target directory.</param>
    /// <returns>True if successful.</returns>
    public static bool SaveToDirectory(this ISitemap sitemap, string targetSitemapDirectory)
    {
        var sitemapGenerator = new SitemapGenerator();
        sitemapGenerator.GenerateSitemaps(sitemap, targetSitemapDirectory);
        
        return true;
    }

    /// <summary>
    /// Asynchronously saves the ISitemap to a file.
    /// </summary>
    /// <param name="sitemap">The ISitemap object.</param>
    /// <param name="path">The file path.</param>
    /// <returns>True if successful.</returns>
    public static async Task<bool> SaveAsync(this ISitemap sitemap, string path)
    {
        try
        {
            var fileSystemWrapper = new FileSystemWrapper();
            var serializer = new SitemapSerializer();
            var xml = serializer.Serialize(sitemap);
            
            var result = await fileSystemWrapper.WriteFileAsync(xml, path);
            
            return result.Exists;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the ISitemap to a file.
    /// </summary>
    /// <param name="sitemap">The ISitemap object.</param>
    /// <param name="path">The file path.</param>
    /// <returns>True if successful.</returns>
    public static bool Save(this ISitemap sitemap, string path)
    {
        try
        {
            var fileSystemWrapper = new FileSystemWrapper();
            var serializer = new SitemapSerializer();
            var xml = serializer.Serialize(sitemap);
            
            var result = fileSystemWrapper.WriteFile(xml, path);
            
            return result.Exists;
        }
        catch
        {
            return false;
        }
    }

    // Internal overloads used for deterministic unit testing. Not part of public API.
    internal static async Task<bool> SaveAsync(this ISitemap sitemap, string path, IFileSystemWrapper fileSystemWrapper)
    {
        try
        {
            var serializer = new SitemapSerializer();
            var xml = serializer.Serialize(sitemap);

            var result = await fileSystemWrapper.WriteFileAsync(xml, path);

            return result.Exists;
        }
        catch
        {
            return false;
        }
    }

    internal static bool Save(this ISitemap sitemap, string path, IFileSystemWrapper fileSystemWrapper)
    {
        try
        {
            var serializer = new SitemapSerializer();
            var xml = serializer.Serialize(sitemap);

            var result = fileSystemWrapper.WriteFile(xml, path);

            return result.Exists;
        }
        catch
        {
            return false;
        }
    }
}