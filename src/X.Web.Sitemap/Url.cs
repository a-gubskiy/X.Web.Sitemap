﻿using System;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
[Serializable]
[XmlRoot("url")]
[XmlType("url")]
public class Url
{
    [XmlElement("loc")]
    public string Location { get; set; }

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
    }

    public static Url CreateUrl(string location) => CreateUrl(location, DateTime.Now);

    public static Url CreateUrl(string url, DateTime timeStamp) =>
        new Url
        {
            Location = url,
            ChangeFrequency = ChangeFrequency.Daily,
            Priority = 0.5d,
            TimeStamp = timeStamp,
        };
}