using System.IO;

namespace X.Web.Sitemap
{
    internal class FileSystemWrapper : IFileSystemWrapper
    {
        public bool DirectoryExists(string pathToDirectory)
        {
            return new DirectoryInfo(pathToDirectory).Exists;
        }

        public FileInfo WriteFile(string xmlString, DirectoryInfo targetDirectory, string targetFileName)
        {
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            var fullPath = Path.Combine(targetDirectory.FullName, targetFileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            File.WriteAllText(fullPath, xmlString);
            return new FileInfo(fullPath);
        }
    }
}