using System;
using System.IO;

namespace X.Web.Sitemap;

internal interface ISitemapXmlSaver
{
    FileInfo SerializeAndSave(Sitemap sitemap, DirectoryInfo targetDirectory, string targetFileName);
}

internal class SitemapXmlSaver : ISitemapXmlSaver
{
    private readonly IFileSystemWrapper _fileSystemWrapper;
    private readonly SitemapSerializer _serializer;

    public SitemapXmlSaver(IFileSystemWrapper fileSystemWrapper)
    {
        _serializer = new SitemapSerializer();
        _fileSystemWrapper = fileSystemWrapper;
    }

    public FileInfo SerializeAndSave(Sitemap sitemap, DirectoryInfo targetDirectory, string targetFileName)
    {
        if (sitemap == null)
        {
            throw new ArgumentNullException(nameof(sitemap));
        }
        
        var xml = _serializer.Serialize(sitemap);
        var path = Path.Combine(targetDirectory.FullName, targetFileName);
                
        return _fileSystemWrapper.WriteFile(xml, path);
    }
}