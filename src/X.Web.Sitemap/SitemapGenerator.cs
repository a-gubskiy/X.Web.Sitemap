﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace X.Web.Sitemap
{
    [PublicAPI]
    public interface ISitemapGenerator
    {
        /// <summary>
        /// Creates one or more sitemaps based on the number of Urls passed in. As of 2016, the maximum number of
        /// urls per sitemap is 50,000 and the maximum file size is 50MB. See https://www.sitemaps.org/protocol.html
        /// for current standards. Filenames will be sitemap-001.xml, sitemap-002.xml, etc.
        /// Returns a list of FileInfo objects for each sitemap that was created (e.g. for subsequent use in generating
        /// a sitemap index file)
        /// </summary>
        /// <param name="urls">
        /// Urls to include in the sitemap(s). If the number of Urls exceeds 50,000 or the file size exceeds 50MB,
        /// then multiple files
        /// will be generated and multiple SitemapInfo objects will be returned.
        /// </param>
        /// <param name="targetDirectory">
        /// The directory where the sitemap(s) will be saved.
        /// </param>
        /// <param name="sitemapBaseFileNameWithoutExtension">
        /// The base file name of the sitemap. For example, if you pick 'products' then it will generate
        /// files with names like products-001.xml, products-002.xml, etc.
        /// </param>
        List<FileInfo> GenerateSitemaps(
            ICollection<Url> urls, 
            DirectoryInfo targetDirectory, 
            string sitemapBaseFileNameWithoutExtension = "sitemap");
    }
    
    public class SitemapGenerator : ISitemapGenerator
    {
        private readonly ISerializedXmlSaver<Sitemap> _serializedXmlSaver;
        
        public SitemapGenerator()
        {
            _serializedXmlSaver = new SerializedXmlSaver<Sitemap>(new FileSystemWrapper());
        }

        internal SitemapGenerator(ISerializedXmlSaver<Sitemap> serializedXmlSaver)
        {
            _serializedXmlSaver = serializedXmlSaver;
        }

        public List<FileInfo> GenerateSitemaps(ICollection<Url> urls, DirectoryInfo targetDirectory, string sitemapBaseFileNameWithoutExtension = "sitemap")
        {
            var sitemaps = BuildSitemaps(urls);

            var sitemapFileInfos = SaveSitemaps(targetDirectory, sitemapBaseFileNameWithoutExtension, sitemaps);

            return sitemapFileInfos;
        }

        private static List<Sitemap> BuildSitemaps(ICollection<Url> urls)
        {
            var sitemaps = new List<Sitemap>();
            var sitemap = new Sitemap();
            var numberOfUrls = urls.Count;
            
            for (var i = 0; i < numberOfUrls; i++)
            {
                if (i % Sitemap.DefaultMaxNumberOfUrlsPerSitemap == 0)
                {
                    sitemap = new Sitemap();
                    sitemaps.Add(sitemap);
                }

                sitemap.Add(urls.ElementAt(i));
            }
            
            return sitemaps;
        }


        private List<FileInfo> SaveSitemaps(DirectoryInfo targetDirectory, string sitemapBaseFileNameWithoutExtension, IReadOnlyList<Sitemap> sitemaps)
        {
            var files = new List<FileInfo>();
            
            for (var i = 0; i < sitemaps.Count; i++)
            {
                var fileName = $"{sitemapBaseFileNameWithoutExtension}-00{i + 1}.xml";
                files.Add(_serializedXmlSaver.SerializeAndSave(sitemaps[i], targetDirectory, fileName));
            }
            
            return files;
        }
    }
}
