using System.IO;
using System.Threading.Tasks;
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

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="sitemap"></param>
    /// <param name="targetSitemapDirectory"></param>
    /// <returns></returns>
    public static bool SaveToDirectory(this ISitemap sitemap, string targetSitemapDirectory)
    {
        var sitemapGenerator = new SitemapGenerator();
        
        // generate one or more sitemaps (depending on the number of URLs) in the designated location.
        sitemapGenerator.GenerateSitemaps(sitemap, targetSitemapDirectory);
        
        return true;
    }
    
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
}