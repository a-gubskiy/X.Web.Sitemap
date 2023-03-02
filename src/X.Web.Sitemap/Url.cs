using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
[Serializable]
[XmlRoot(ElementName = "image", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
public class Image
{
    [XmlElement(ElementName = "loc", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string Location { get; set; }
}

[PublicAPI]
[Serializable]
[XmlRoot("url")]
[XmlType("url")]
public class Url
{
    [XmlElement("loc")]
    public string Location { get; set; }
    
    [XmlElement(ElementName = "image", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public List<Image> Images { get; set; }

    [XmlIgnore]
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Please do not use this property to change last modification date. 
    /// Use TimeStamp instead.
    /// </summary>
    [XmlElement("lastmod")]
    public string LastMod
    {
        get => TimeStamp.ToString("yyyy-MM-ddTHH:mm:sszzz");
        set => TimeStamp = DateTime.Parse(value);
    }

    [XmlElement("changefreq")]
    public ChangeFrequency ChangeFrequency { get; set; }

    [XmlElement("priority")]
    public double Priority { get; set; }

    public Url()
    {
        Location = "";
    }

    public static Url CreateUrl(string location) => CreateUrl(location, DateTime.Now);

    public static Url CreateUrl(string url, DateTime timeStamp) =>
        new()
        {
            Location = url,
            ChangeFrequency = ChangeFrequency.Daily,
            Priority = 0.5d,
            TimeStamp = timeStamp,
        };
}