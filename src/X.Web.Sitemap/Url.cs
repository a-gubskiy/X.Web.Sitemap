using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace X.Web.Sitemap;

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
    public DateTime? TimeStamp { get; set; }

    /// <summary>
    /// Please do not use this property to change last modification date.
    /// Use TimeStamp instead.
    /// </summary>
    [XmlElement("lastmod")]
    public string LastMod
    {
        get => TimeStamp?.ToString("yyyy-MM-ddTHH:mm:sszzz") ?? "";
        set => TimeStamp = string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value);
    }

    /// <summary>
    /// Change frequency of the page.
    /// </summary>
    [XmlElement("changefreq")]
    public ChangeFrequency? ChangeFrequency { get; set; }

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
    /// Creates a new URL object with the specified location, timestamp, change frequency, and priority.
    /// </summary>
    /// <param name="url">The URL of the page. This will be set as the Location property.</param>
    /// <param name="timeStamp">The time of last modification for the page.</param>
    /// <param name="changeFrequency">Optional change frequency hint for crawlers indicating how often the page is likely to change. Defaults to null.</param>
    /// <param name="priority">The priority of the URL relative to other URLs on the site, ranging from 0.0 to 1.0. Defaults to 0.5.</param>
    /// <returns>A new <see cref="Url"/> instance initialized with the specified parameters.</returns>
    /// <remarks>
    /// This factory method provides a convenient way to create URL entries for XML sitemaps.
    /// The priority value should be between 0.0 and 1.0, where higher values indicate higher priority.
    /// </remarks>
    public static Url CreateUrl(
        string url,
        DateTime timeStamp,
        ChangeFrequency? changeFrequency = null,
        double priority = 0.5d)
    {
        if (priority < 0.0d || priority > 1.0d)
        {
            throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be between 0.0 and 1.0.");
        }

        return new()
        {
            Location = url,
            ChangeFrequency = changeFrequency,
            Priority = priority,
            TimeStamp = timeStamp,
        };
    }
}