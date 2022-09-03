using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using X.Web.Sitemap.Extensions;

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

    public virtual string ToXml()
    {
        var serializer = new XmlSerializer(typeof(Sitemap));

        using (var writer = new StringWriterUtf8())
        {
            serializer.Serialize(writer, this);
            return writer.ToString();
        }
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

    /// <summary>
    /// Generate multiple sitemap files
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public virtual bool SaveToDirectory(string directory)
    {
        try
        {
            var parts = Count % MaxNumberOfUrlsPerSitemap == 0
                ? Count / MaxNumberOfUrlsPerSitemap
                : (Count / MaxNumberOfUrlsPerSitemap) + 1;
                
            var xmlDocument = new XmlDocument();
                
            xmlDocument.LoadXml(ToXml());
                    
            var all = xmlDocument.ChildNodes[1].ChildNodes.Cast<XmlNode>().ToList();

            for (var i = 0; i < parts; i++)
            {
                var take = MaxNumberOfUrlsPerSitemap * i;
                var top = all.Take(take).ToList();
                var bottom = all.Skip(take + MaxNumberOfUrlsPerSitemap).Take(Count - take - MaxNumberOfUrlsPerSitemap).ToList();

                var nodes = new List<XmlNode>();
                    
                nodes.AddRange(top);
                nodes.AddRange(bottom);

                foreach (var node in nodes)
                {
                    if (node.ParentNode != null)
                    {
                        node.ParentNode.RemoveChild(node);
                    }
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

    public static Sitemap Parse(string xml)
    {
        using (TextReader textReader = new StringReader(xml))
        {
            var serializer = new XmlSerializer(typeof(Sitemap));
            return (Sitemap)serializer.Deserialize(textReader);
        }
    }

    public static bool TryParse(string xml, out Sitemap? sitemap)
    {
        try
        {
            sitemap = Parse(xml);
            return true;
        }
        catch
        {
            sitemap = null;
            return false;
        }
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