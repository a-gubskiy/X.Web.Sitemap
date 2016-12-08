using System.IO;

namespace X.Web.Sitemap
{
    public interface IFileSystemWrapper
    {
        bool DirectoryExists(string pathToDirectory);
        void WriteFile(string xmlString, DirectoryInfo targetDirectory, string targetFileName);
    }
}
