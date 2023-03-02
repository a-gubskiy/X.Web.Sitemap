using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
public interface ISitemapIndexGenerator
{
	/// <summary>
	/// Creates a sitemap index file for the specified sitemaps.
	/// </summary>
	/// <param name="sitemaps">The sitemaps in include in the sitemap index.</param>
	/// <param name="targetDirectory">The path to the directory where you'd like the sitemap index file to be written. (e.g. "C:\sitemaps\" or "\\myserver\sitemaplocation\".</param>
	/// <param name="targetSitemapIndexFileName">The name of the sitemap to be generated (e.g. "sitemapindex.xml")</param>
	SitemapIndex GenerateSitemapIndex(IEnumerable<SitemapInfo> sitemaps, DirectoryInfo targetDirectory, string targetSitemapIndexFileName);

	/// <summary>
	/// Creates a sitemap index file for the specified sitemaps.
	/// </summary>
	/// <param name="sitemaps">The sitemaps in include in the sitemap index.</param>
	/// <param name="targetDirectory">The path to the directory where you'd like the sitemap index file to be written. (e.g. "C:\sitemaps\" or "\\myserver\sitemaplocation\".</param>
	/// <param name="targetSitemapIndexFileName">The name of the sitemap to be generated (e.g. "sitemapindex.xml")</param>
	SitemapIndex GenerateSitemapIndex(IEnumerable<SitemapInfo> sitemaps, string targetDirectory, string targetSitemapIndexFileName);
}

[PublicAPI]
public class SitemapIndexGenerator : ISitemapIndexGenerator
{
	private readonly ISerializedXmlSaver<SitemapIndex> _serializedXmlSaver;

	public SitemapIndexGenerator()
	{
		_serializedXmlSaver = new SerializedXmlSaver<SitemapIndex>(new FileSystemWrapper());
	}

	internal SitemapIndexGenerator(ISerializedXmlSaver<SitemapIndex> serializedXmlSaver)
	{
		_serializedXmlSaver = serializedXmlSaver;
	}

	public SitemapIndex GenerateSitemapIndex(IEnumerable<SitemapInfo> sitemaps, string targetDirectory, string targetSitemapFileName) => 
		GenerateSitemapIndex(sitemaps, new DirectoryInfo(targetDirectory), targetSitemapFileName);

	public SitemapIndex GenerateSitemapIndex(IEnumerable<SitemapInfo> sitemaps, DirectoryInfo targetDirectory, string targetSitemapFileName)
	{
		var sitemapIndex = new SitemapIndex(sitemaps);
		_serializedXmlSaver.SerializeAndSave(sitemapIndex, targetDirectory, targetSitemapFileName);
		return sitemapIndex;
	}
}