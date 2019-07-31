using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("X.Web.Sitemap.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace X.Web.Sitemap
{
    [Serializable]
    [XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap : List<Url>, ISitemap
    {
        private const int LineCount = 1000;

        public virtual string ToXml()
        {
            var xmlSerializer = new XmlSerializer(typeof(Sitemap));

            using (var textWriter = new StringWriterUtf8())
            {
                xmlSerializer.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        public virtual async Task<bool> SaveAsync(string path)
        {
            var directory = Path.GetDirectoryName(path);
            EnsureDirectoryCreated(directory);

            try
            {
                using (var file = new FileStream(path, FileMode.Create))
                using (var writer = new StreamWriter(file))
                {
                    await writer.WriteAsync(ToXml());
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool Save(string path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);

                if (directory != null)
                {
                    EnsureDirectoryCreated(directory);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    File.WriteAllText(path, ToXml());

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Generate multiple sitemap files
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public virtual bool SaveToDirectory(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var xml = ToXml();

                var parts = Count % LineCount == 0
                    ? Count / LineCount
                    : (Count / LineCount) + 1;

                for (var i = 0; i < parts; i++)
                {
                    var fileName = string.Format("sitemap{0}.xml", i);
                    var path = Path.Combine(directory, fileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    var xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xml);

                    var take = LineCount * i;

                    var all = xmlDocument.ChildNodes[1].ChildNodes.Cast<XmlNode>().ToList();

                    var top = all.Take(take).ToList();
                    var bottom = all.Skip(take + LineCount).Take(Count - take - LineCount).ToList();

                    var nodes = new List<XmlNode>();
                    nodes.AddRange(top);
                    nodes.AddRange(bottom);

                    foreach (var node in nodes)
                    {
                        node.ParentNode.RemoveChild(node);
                    }

                    using (var writer = File.CreateText(path))
                    {
                        xmlDocument.Save(writer);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Sitemap Parse(string xml)
        {
            using(TextReader textReader = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Sitemap));
                var sitemap = serializer.Deserialize(textReader);
                return sitemap as Sitemap;
            }
        }

        public static bool TryParse(string xml, out Sitemap sitemap)
        {
            try
            {
                sitemap = Parse(xml);
                return true;
            }
            catch
            {
                sitemap = null;
                return false;
            }
        }

        private void EnsureDirectoryCreated(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }

    /// <summary>
    /// Subclass the StringWriter class and override the default encoding.  
    /// This allows us to produce XML encoded as UTF-8. 
    /// </summary>
    public class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}