using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap;

public interface ISitemapSerializer
{
    int MaxNumberOfUrlsPerSitemap { get; set; }
    string ToXml(ISitemapBase sitemap);
    Task<bool> SaveAsync(string path, ISitemapBase sitemap);
    bool Save(string path, ISitemapBase sitemap);

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="sitemap"></param>
    /// <returns></returns>
    bool SaveToDirectory(string directory, ISitemapBase sitemap);
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


    public virtual string ToXml(ISitemapBase sitemap)
    {
        var serializer = new XmlSerializer(typeof(Sitemap));

        using (var writer = new StringWriterUtf8())
        {
            serializer.Serialize(writer, sitemap);
            return writer.ToString();
        }
    }

    public virtual async Task<bool> SaveAsync(string path, ISitemapBase sitemap)
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

    public virtual bool Save(string path, ISitemapBase sitemap)
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
    public virtual bool SaveToDirectory(string directory, ISitemapBase sitemap)
    {
        try
        {
            var parts = sitemap.Count() % MaxNumberOfUrlsPerSitemap == 0
                ? sitemap.Count() / MaxNumberOfUrlsPerSitemap
                : (sitemap.Count() / MaxNumberOfUrlsPerSitemap) + 1;

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(ToXml(sitemap));

            var all = xmlDocument.ChildNodes[1].ChildNodes.Cast<XmlNode>().ToList();

            for (var i = 0; i < parts; i++)
            {
                var take = MaxNumberOfUrlsPerSitemap * i;
                var top = all.Take(take).ToList();
                var bottom = all.Skip(take + MaxNumberOfUrlsPerSitemap)
                    .Take(sitemap.Count() - take - MaxNumberOfUrlsPerSitemap).ToList();

                var nodes = new List<XmlNode>();

                nodes.AddRange(top);
                nodes.AddRange(bottom);

                foreach (var node in nodes)
                {
                    node.ParentNode.RemoveChild(node);
                }

                _fileSystemWrapper.WriteFile(xmlDocument.ToXmlString(), Path.Combine(directory, $"sitemap{i}.xml"));
            }

            return true;
        }
        catch
        {
            return false;
        }
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
            return true;
        }
        catch
        {
            sitemap = null;
            return false;
        }
    }
}
