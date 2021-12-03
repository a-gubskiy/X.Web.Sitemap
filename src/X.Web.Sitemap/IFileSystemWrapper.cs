using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
internal interface IFileSystemWrapper
{
    FileInfo WriteFile(string xml, string path);
        
    Task<FileInfo> WriteFileAsync(string xml, string path);
}