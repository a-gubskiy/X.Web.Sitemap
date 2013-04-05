using System;
using System.Xml.Serialization;

namespace X.Web.Sitemap
{
    [Serializable]
    [XmlRoot("url")]
    [XmlType("url")]
    public class Url
    {
        [XmlElement("loc")]
        public String Location { get; set; }

        [XmlElement("lastmod")]
        public DateTime TimeStamp { get; set; }

        [XmlElement("changefreq")]
        public ChangeFrequency ChangeFrequency { get; set; }

        [XmlElement("priority")]
        public double Priority { get; set; }

        public Url()
        {
        }

        public static Url CreateUrl(string location)
        {
            return CreateUrl(location, DateTime.Now);
        }

        public static Url CreateUrl(string url, DateTime timeStamp)
        {
            return new Url
                       {
                           Location = url,
                           ChangeFrequency = ChangeFrequency.Daily,
                           Priority = 0.5d,
                           TimeStamp = timeStamp,
                       };
        }
    }
}
