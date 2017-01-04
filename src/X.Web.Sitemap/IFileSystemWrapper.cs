using System.IO;

namespace X.Web.Sitemap
{
    internal interface IFileSystemWrapper
    {
        bool DirectoryExists(string pathToDirectory);
        FileInfo WriteFile(string xmlString, DirectoryInfo targetDirectory, string targetFileName);
    }
}
