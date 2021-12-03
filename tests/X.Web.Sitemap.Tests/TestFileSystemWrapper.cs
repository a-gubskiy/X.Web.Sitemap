using System.IO;
using System.Threading.Tasks;

namespace X.Web.Sitemap.Tests;

public class TestFileSystemWrapper : IFileSystemWrapper
{
	public FileInfo WriteFile(string xml, string path)
	{
		return new FileInfo(path);
	}

	public Task<FileInfo> WriteFileAsync(string xml, string path)
	{
		return Task.FromResult(WriteFile(xml, path));
	}
}