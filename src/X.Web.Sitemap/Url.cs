using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
[Serializable]
[Description("Encloses all information about a single image. Each URL (<loc> tag) can include up to 1,000 <image:image> tags.")]
[XmlRoot(ElementName = "image", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
public class Image
{
    [Description("The URL of the image.")]
    [XmlElement(ElementName = "loc", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string Location { get; set; } = "";

    [Description("The caption of the image.")]
    [XmlElement(ElementName = "caption", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? Caption { get; set; }

    [Description("The geographic location of the image. For example, \"Limerick, Ireland\".")]
    [XmlElement(ElementName = "geo_location", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? GeographicLocation { get; set; }

    [Description("The title of the image.")]
    [XmlElement(ElementName = "title", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? Title { get; set; }

    [Description("A URL to the license of the image.")]
    [XmlElement(ElementName = "license", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? License { get; set; }
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
        Images = new List<Image>();
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