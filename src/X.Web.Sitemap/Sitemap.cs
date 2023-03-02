using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("X.Web.Sitemap.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace X.Web.Sitemap;

[PublicAPI]
[Serializable]
[XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public class Sitemap : List<Url>, ISitemap
{
    private readonly IFileSystemWrapper _fileSystemWrapper;

    public static int DefaultMaxNumberOfUrlsPerSitemap = 5000;
        
    public int MaxNumberOfUrlsPerSitemap { get; set; }

    public Sitemap()
    {
        _fileSystemWrapper = new FileSystemWrapper();
        MaxNumberOfUrlsPerSitemap = DefaultMaxNumberOfUrlsPerSitemap;
    }

    public Sitemap(IEnumerable<Url> urls) : this() => AddRange(urls);

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="targetSitemapDirectory"></param>
    /// <returns></returns>
    public virtual bool SaveToDirectory(string targetSitemapDirectory)
    {
        var sitemapGenerator = new SitemapGenerator();
        
        // generate one or more sitemaps (depending on the number of URLs) in the designated location.
        sitemapGenerator.GenerateSitemaps(this, targetSitemapDirectory);
        
        return true;
    }

    public virtual string ToXml()
    {
        var serializer = new SitemapSerializer();
        return serializer.Serialize(this);
    }

    public virtual async Task<bool> SaveAsync(string path)
    {
        try
        {
            var result = await _fileSystemWrapper.WriteFileAsync(ToXml(), path);
            return result.Exists;
        }
        catch
        {
            return false;
        }
    }

    public virtual bool Save(string path)
    {
        try
        {
            var result = _fileSystemWrapper.WriteFile(ToXml(), path);
            return result.Exists;
        }
        catch
        {
            return false;
        }
    }

    public static Sitemap Parse(string xml) => SitemapSerializer.Deserialize(xml);

    public static bool TryParse(string xml, out Sitemap? sitemap)
    {
        try
        {
            sitemap = Parse(xml);
        }
        catch
        {
            sitemap = null;
        }
        
        return sitemap != null;
    }
}

/// <summary>
/// Subclass the StringWriter class and override the default encoding.  
/// This allows us to produce XML encoded as UTF-8. 
/// </summary>
public class StringWriterUtf8 : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}