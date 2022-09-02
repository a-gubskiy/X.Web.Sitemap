namespace X.Web.Sitemap;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Extensions;

//https://github.com/ernado-x/X.Web.Sitemap/issues/33
public static class XSitemapSaveToDirectoryFixed
{
    private static List<List<XmlNode>> GetAllNodesChunked(Sitemap sitemap)
    {
        var sitemapMaxNumberOfUrlsPerSitemap = sitemap.MaxNumberOfUrlsPerSitemap;

        XmlDocument xmlDocument = new();
        xmlDocument.LoadXml(sitemap.ToXml());
        var allXmlNodes = xmlDocument.DocumentElement?.ChildNodes.Cast<XmlNode>().ToList() ?? new();

        List<List<XmlNode>> allNodesChunked = ChunkBy(allXmlNodes, sitemapMaxNumberOfUrlsPerSitemap);
        return allNodesChunked;
    }

    //https://stackoverflow.com/a/24087164/828184
    private static List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
    {
        var chunkedList = source
            .Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList())
            .ToList();

        return chunkedList;
    }

    //the original method has an error: https://github.com/ernado-x/X.Web.Sitemap/issues/33
    public static List<FileInfo> SaveToDirectory(Sitemap sitemap, string directory)
    {
        var createdFiles = new List<FileInfo>();

        var fileSystemWrapper = (IFileSystemWrapper)new FileSystemWrapper();

        var allNodesChunked = GetAllNodesChunked(sitemap);

        var fileCounter = 0;
        foreach (var chunkedNodes in allNodesChunked)
        {
            XmlDocument xmlDocument = new();
            xmlDocument.LoadXml(new Sitemap().ToXml());

            var filename = $"sitemap{fileCounter}.xml";

            List<XmlNode> xmlNodeList = new();
            xmlNodeList.AddRange(chunkedNodes);
            foreach (var xmlNode in xmlNodeList)
            {
                //node needs to be imported or wrong context exception
                var importedNode = xmlDocument.ImportNode(xmlNode, true);

                xmlDocument.DocumentElement?.AppendChild(importedNode);
            }

            var fileInfo = fileSystemWrapper.WriteFile(xmlDocument.ToXmlString(), Path.Combine(directory, filename));
            createdFiles.Add(fileInfo);

            fileCounter++;
        }

        return createdFiles;
    }
}