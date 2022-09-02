using System;
using System.IO;
using System.Threading.Tasks;

namespace X.Web.Sitemap;

internal class FileSystemWrapper : IFileSystemWrapper
{
    public FileInfo WriteFile(string xml, string path)
    {
        var directory = Path.GetDirectoryName(path);
            
        EnsureDirectoryCreated(directory);
            
        using (var file = new FileStream(path, FileMode.Create))
        using (var writer = new StreamWriter(file))
        {
            writer.Write(xml);
        }
            
        return new FileInfo(path);
    }

    public async Task<FileInfo> WriteFileAsync(string xml, string path)
    {
        var directory = Path.GetDirectoryName(path);
            
        EnsureDirectoryCreated(directory);
            
        using (var file = new FileStream(path, FileMode.Create))
        using (var writer = new StreamWriter(file))
        {
            await writer.WriteAsync(xml);
        }
            
        return new FileInfo(path);
    }

    private static void EnsureDirectoryCreated(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}