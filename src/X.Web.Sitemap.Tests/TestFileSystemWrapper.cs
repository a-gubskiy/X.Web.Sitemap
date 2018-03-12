using System.IO;

namespace X.Web.Sitemap.Tests
{
	public class TestFileSystemWrapper : IFileSystemWrapper
	{
		public bool DirectoryExists(string pathToDirectory)
		{
			return true;
		}

		public FileInfo WriteFile(string xmlString, DirectoryInfo targetDirectory, string targetFileName)
		{
			var file = new FileInfo(Path.Combine(targetDirectory.FullName, targetFileName));		
			return file;
		}
	}
}
