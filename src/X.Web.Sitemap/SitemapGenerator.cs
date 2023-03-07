using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

public class SitemapGenerator : ISitemapGenerator
{
    private readonly ISerializedXmlSaver<Sitemap> _serializedXmlSaver;

    [PublicAPI]
    public int MaxNumberOfUrlsPerSitemap { get; set; } = Sitemap.DefaultMaxNumberOfUrlsPerSitemap;
        
    public SitemapGenerator()
    {
        _serializedXmlSaver = new SerializedXmlSaver<Sitemap>(new FileSystemWrapper());
    }

    internal SitemapGenerator(ISerializedXmlSaver<Sitemap> serializedXmlSaver)
    {
        _serializedXmlSaver = serializedXmlSaver;
    }

    public List<FileInfo> GenerateSitemaps(IEnumerable<Url> urls, string targetDirectory, string sitemapBaseFileNameWithoutExtension = "sitemap") => 
        GenerateSitemaps(urls, new DirectoryInfo(targetDirectory), sitemapBaseFileNameWithoutExtension);

    public List<FileInfo> GenerateSitemaps(IEnumerable<Url> urls, DirectoryInfo targetDirectory, string sitemapBaseFileNameWithoutExtension = "sitemap")
    {
        var sitemaps = BuildSitemaps(urls.ToList(), MaxNumberOfUrlsPerSitemap);

        var sitemapFileInfos = SaveSitemaps(targetDirectory, sitemapBaseFileNameWithoutExtension, sitemaps);

        return sitemapFileInfos;
    }

    private static List<Sitemap> BuildSitemaps(IReadOnlyList<Url> urls, int maxNumberOfUrlsPerSitemap)
    {
        var sitemaps = new List<Sitemap>();
        var sitemap = new Sitemap();
        var numberOfUrls = urls.Count;
            
        for (var i = 0; i < numberOfUrls; i++)
        {
            if (i % maxNumberOfUrlsPerSitemap == 0)
            {
                sitemap = new Sitemap();
                sitemaps.Add(sitemap);
            }

            sitemap.Add(urls[i]);
        }
            
        return sitemaps;
    }

    private List<FileInfo> SaveSitemaps(DirectoryInfo targetDirectory, string sitemapBaseFileNameWithoutExtension, IReadOnlyList<Sitemap> sitemaps)
    {
        var files = new List<FileInfo>();
            
        for (var i = 0; i < sitemaps.Count; i++)
        {
            var fileName = $"{sitemapBaseFileNameWithoutExtension}-{i + 1}.xml";
            files.Add(_serializedXmlSaver.SerializeAndSave(sitemaps[i], targetDirectory, fileName));
        }
            
        return files;
    }
}