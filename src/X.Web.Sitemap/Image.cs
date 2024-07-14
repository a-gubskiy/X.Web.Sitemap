using System;
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
    /// <summary>
    /// Location of the image.
    /// </summary>
    [Description("The URL of the image.")]
    [XmlElement(ElementName = "loc", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string Location { get; set; } = "";

    /// <summary>
    /// Caption of the image.
    /// </summary>
    [Description("The caption of the image.")]
    [XmlElement(ElementName = "caption", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? Caption { get; set; }

    /// <summary>
    /// Geographic location of the image.
    /// </summary>
    [Description("The geographic location of the image. For example, \"Limerick, Ireland\".")]
    [XmlElement(ElementName = "geo_location", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? GeographicLocation { get; set; }

    /// <summary>
    /// Title of the image.
    /// </summary>
    [Description("The title of the image.")]
    [XmlElement(ElementName = "title", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? Title { get; set; }

    /// <summary>
    /// License of the image.
    /// </summary>
    [Description("A URL to the license of the image.")]
    [XmlElement(ElementName = "license", Namespace = "http://www.google.com/schemas/sitemap-image/1.1")]
    public string? License { get; set; }
}