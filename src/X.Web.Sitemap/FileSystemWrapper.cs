using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
internal interface IFileSystemWrapper
{
    /// <summary>
    /// Writes the specified XML to the specified path.
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    FileInfo WriteFile(string xml, string path);
        
    /// <summary>
    /// Writes the specified XML to the specified path asynchronously.
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    Task<FileInfo> WriteFileAsync(string xml, string path);
}

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

    private static void EnsureDirectoryCreated(string? directory)
    {
        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentException(nameof(directory));
        }
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}