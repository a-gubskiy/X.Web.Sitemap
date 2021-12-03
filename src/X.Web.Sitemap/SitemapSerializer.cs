using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap;

[PublicAPI]
public interface ISitemapSerializer
{
    int MaxNumberOfUrlsPerSitemap { get; set; }

    string ToXml<T>(T sitemap) where T : class, ISitemapBase;

    Task<bool> SaveAsync<T>(string path, T sitemap) where T : class, ISitemapBase;

    bool Save<T>(string path, T sitemap) where T : class, ISitemapBase;

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="sitemap"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool SaveToDirectory<T>(string directory, T sitemap) where T : class, ISitemapBase;

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="sitemap"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SaveToDirectoryAsync<T>(string directory, T sitemap) where T : class, ISitemapBase;
}

public class SitemapSerializer : ISitemapSerializer
{
    private readonly IFileSystemWrapper _fileSystemWrapper;

    public static int DefaultMaxNumberOfUrlsPerSitemap = 5000;

    public int MaxNumberOfUrlsPerSitemap { get; set; }

    public SitemapSerializer()
    {
        _fileSystemWrapper = new FileSystemWrapper();

        MaxNumberOfUrlsPerSitemap = DefaultMaxNumberOfUrlsPerSitemap;
    }

    public virtual string ToXml<T>(T sitemap) where T : class, ISitemapBase
    {
        var serializer = new XmlSerializer(typeof(T));

        using (var writer = new StringWriterUtf8())
        {
            serializer.Serialize(writer, sitemap);
            return writer.ToString();
        }
    }

    public virtual async Task<bool> SaveAsync<T>(string path, T sitemap) where T : class, ISitemapBase
    {
        try
        {
            return await _fileSystemWrapper.WriteFileAsync(ToXml(sitemap), path) != null;
        }
        catch
        {
            return false;
        }
    }

    public virtual bool Save<T>(string path, T sitemap) where T : class, ISitemapBase
    {
        try
        {
            return _fileSystemWrapper.WriteFile(ToXml(sitemap), path) != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="sitemap"></param>
    /// <returns></returns>
    public virtual bool SaveToDirectory<T>(string directory, T sitemap) where T : class, ISitemapBase
    {
        try
        {
            var parts = CalculateParts(sitemap);
            var documentXml = ToXml(sitemap);
            
            for (var i = 0; i < parts; i++)
            {
                var xml = GeneratePartialDocumentXml(sitemap, documentXml, i);
                _fileSystemWrapper.WriteFile(xml, Path.Combine(directory, $"sitemap{i}.xml"));
            }
        }
        catch
        {
            return false;
        }
        

        return true;
    }

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="sitemap"></param>
    /// <returns></returns>
    public virtual async Task SaveToDirectoryAsync<T>(string directory, T sitemap) where T : class, ISitemapBase
    {
        var parts = CalculateParts(sitemap);
        var documentXml = ToXml(sitemap);
            
        for (var i = 0; i < parts; i++)
        {
            var xml = GeneratePartialDocumentXml(sitemap, documentXml, i);
            await _fileSystemWrapper.WriteFileAsync(xml, Path.Combine(directory, $"sitemap{i}.xml"));
        }
    }

    private string GeneratePartialDocumentXml<T>(T sitemap, string documentXml, int i) where T : class, ISitemapBase
    {
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(documentXml);
        
        var all = xmlDocument.ChildNodes[1].ChildNodes.Cast<XmlNode>().ToList();
        var take = MaxNumberOfUrlsPerSitemap * i;
        var top = all.Take(take).ToList();
        var bottom = all
            .Skip(take + MaxNumberOfUrlsPerSitemap)
            .Take(sitemap.Count() - take - MaxNumberOfUrlsPerSitemap)
            .ToList();

        var nodes = new List<XmlNode>();

        nodes.AddRange(top);
        nodes.AddRange(bottom);

        foreach (var node in nodes)
        {
            node.ParentNode?.RemoveChild(node);
        }

        return xmlDocument.ToXmlString();
    }

    private int CalculateParts<T>(T sitemap) where T : class, ISitemapBase
    {
        var parts = sitemap.Count() % MaxNumberOfUrlsPerSitemap == 0
            ? sitemap.Count() / MaxNumberOfUrlsPerSitemap
            : (sitemap.Count() / MaxNumberOfUrlsPerSitemap) + 1;
        
        return parts;
    }

    public static T Parse<T>(string xml) where T : class, ISitemapBase
    {
        using (TextReader textReader = new StringReader(xml))
        {
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(textReader) as T;
        }
    }

    public static bool TryParse<T>(string xml, out T sitemap) where T : class, ISitemapBase
    {
        try
        {
            sitemap = Parse<T>(xml);
        }
        catch
        {
            sitemap = null;
            return false;
        }
        
        return true;
    }
}
