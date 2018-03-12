using System.Collections.Generic;
using System.IO;

namespace X.Web.Sitemap
{
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

		public SitemapIndex GenerateSitemapIndex(List<SitemapInfo> sitemaps, DirectoryInfo targetDirectory, string targetSitemapFileName)
		{
			var sitemapIndex = new SitemapIndex(sitemaps);
			_serializedXmlSaver.SerializeAndSave(sitemapIndex, targetDirectory, targetSitemapFileName);
			return sitemapIndex;
		}
	}
}