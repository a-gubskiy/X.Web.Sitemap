using System.IO;
using System.Threading.Tasks;

namespace X.Web.Sitemap
{
    internal interface IFileSystemWrapper
    {
        FileInfo WriteFile(string xml, string path);
        
        Task<FileInfo> WriteFileAsync(string xml, string path);
    }
}
