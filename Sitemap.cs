using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace X.Web.Sitemap
{
    [Serializable]
    public class Sitemap : List<Url>
    {
        public const string MimeType = "text/xml";

        public Sitemap()
        {
        }

        public string ToXml()
        {
            return GetXml(0, this.Count); 
        }

        private string GetXml(int position, int count)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd\">");

            count = position + count > this.Count ? this.Count : position + count;

            for (var i = position; i < count; i++)
            {
                var url = this[i];
                sb.AppendLine(GetXml(url));
            }

            sb.AppendLine("</urlset>");

            var result = sb.ToString().Replace("0,", "0.");

            return result;
        }

        private static string GetXml(Url url)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<url>");
            sb.AppendFormat("<loc>{0}</loc>", url.Location);
            sb.AppendFormat("<lastmod>{0}-{1}-{2}</lastmod>", url.TimeStamp.Year, url.TimeStamp.Month.ToString("00"), url.TimeStamp.Day.ToString("00"));
            sb.AppendFormat("<changefreq>{0}</changefreq>", ToString(url.ChangeFrequency));
            sb.AppendFormat("<priority>{0}</priority>", url.Priority);
            sb.AppendLine("</url>");
            return sb.ToString();
        }

        private static string ToString(ChangeFrequency changeFrequency)
        {
            switch (changeFrequency)
            {
                case ChangeFrequency.Daily: return "daily";
                case ChangeFrequency.Weekly: return "weekly";
            }

            throw new Exception();
        }

        public bool Save(String path)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);

                if (directory != null)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    var streamWriter = new StreamWriter(path);
                    streamWriter.Write(ToXml());
                    streamWriter.Close();

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
        public bool SaveToDirectory(String directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                int parts;
                const int lineCount = 1000;

                if (Count % lineCount == 0)
                {
                    parts = this.Count / lineCount;
                }
                else
                {
                    parts = (Count / lineCount) + 1;
                }

                for (int i = 0; i < parts; i++)
                {
                    var fileName = String.Format("sitemap{0}.xml", i);
                    var path = Path.Combine(directory, fileName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    var streamWriter = new StreamWriter(path);
                    streamWriter.Write(GetXml(i * lineCount, lineCount));
                    streamWriter.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

