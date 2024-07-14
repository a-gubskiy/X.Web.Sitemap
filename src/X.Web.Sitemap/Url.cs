using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
[Serializable]
[XmlRoot("url")]
[XmlType("url")]
public class Url
{
    /// <summary>
    /// Location of the page.
    /// </summary>
    [XmlElement("loc")]
    public string Location { get; set; }

    /// <summary>
    /// Images collection associated with this URL.
    /// </summary>
    [XmlElement(ElementName = "image", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public List<Image> Images { get; set; }

    /// <summary>
    /// Time of last modification.
    /// </summary>
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

    /// <summary>
    /// Change frequency of the page.
    /// </summary>
    [XmlElement("changefreq")]
    public ChangeFrequency ChangeFrequency { get; set; }

    /// <summary>
    /// Priority of the URL relative to other URLs on the site.
    /// </summary>
    [XmlElement("priority")]
    public double Priority { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Url()
    {
        Location = "";
        Images = new List<Image>();
        Location = "";
    }

    /// <summary>
    /// Creates a new URL object with the specified location.
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public static Url CreateUrl(string location) => CreateUrl(location, DateTime.Now);

    /// <summary>
    /// Creates a new URL object with the specified location and timestamp.
    /// </summary>
    /// <param name="url">
    /// URL of the page.
    /// </param>
    /// <param name="timeStamp">
    /// Time of last modification.
    /// </param>
    /// <returns></returns>
    public static Url CreateUrl(string url, DateTime timeStamp) =>
        new()
        {
            Location = url,
            ChangeFrequency = ChangeFrequency.Daily,
            Priority = 0.5d,
            TimeStamp = timeStamp,
        };
}