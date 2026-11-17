# X.Web.Sitemap

[![NuGet Version](https://img.shields.io/nuget/v/X.Web.Sitemap?style=for-the-badge&label=NuGet&color=004880&logo=nuget&logoColor=white)](https://www.nuget.org/packages/X.Web.Sitemap)
[![NuGet Downloads](https://img.shields.io/nuget/dt/X.Web.Sitemap?style=for-the-badge&label=Downloads&color=004880&logo=nuget&logoColor=white)](https://www.nuget.org/packages/X.Web.Sitemap)
[![Sponsor on GitHub](https://img.shields.io/badge/Sponsor_on_GitHub-ff7f00?style=for-the-badge&logo=github&logoColor=white)](https://github.com/sponsors/a-gubskiy)
[![Subscribe on X](https://img.shields.io/badge/Subscribe_on_X-000000?style=for-the-badge&logo=x&logoColor=white)](https://x.com/andrew_gubskiy)

X.Web.Sitemap is a library for generating and managing sitemaps in .NET applications.

## Installation

```bash
dotnet add package X.Web.Sitemap
```

Targets `netstandard2.0`, `net10.0` and `net11.0`.

## Usage example

### Sitemap usage example 
Below is an example of basic usage:

```cs
using X.Web.Sitemap;
using X.Web.Sitemap.Extensions;

var sitemap = new Sitemap();

sitemap.Add(new Url
{
    ChangeFrequency = ChangeFrequency.Hourly,
    Location = "https://www.example.com",
    Priority = 0.8,
    TimeStamp = DateTime.Now
});

sitemap.Add(CreateUrl("https://www.example.com/link1"));
sitemap.Add(CreateUrl("https://www.example.com/link2"));
sitemap.Add(CreateUrl("https://www.example.com/link3"));
sitemap.Add(CreateUrl("https://www.example.com/link4"));
sitemap.Add(CreateUrl("https://www.example.com/link5"));

// Save sitemap structure to file
sitemap.Save("/var/www/example.com/sitemap.xml");

// Split a large list into pieces and store in a directory
sitemap.SaveToDirectory("/var/www/example.com/sitemaps");

// Get xml-content of file
Console.Write(sitemap.ToXml());

static Url CreateUrl(string url) =>
    Url.CreateUrl(url, DateTime.Now, ChangeFrequency.Daily, 0.5);
```

`Save`, `SaveAsync`, `SaveToDirectory`, `ToXml` and `ToStream` are extension methods in the `X.Web.Sitemap.Extensions` namespace.

### SitemapGenerator usage example
Below is a more comprehensive example that demonstrates how to create multiple sitemaps and how to add them to a sitemap index file.
`ISitemapGenerator` and `ISitemapIndexGenerator` live in the `X.Web.Sitemap.Generators` namespace; the default implementations are
`SitemapGenerator` and `SitemapIndexGenerator`, so you can register them in your DI container
(e.g. `services.AddSingleton<ISitemapGenerator, SitemapGenerator>()`) or simply create them with `new SitemapGenerator()` / `new SitemapIndexGenerator()`.

```cs
using X.Web.Sitemap;
using X.Web.Sitemap.Generators;

public class SitemapGenerationWithSitemapIndexExample
{
    private readonly ISitemapGenerator _sitemapGenerator;
    private readonly ISitemapIndexGenerator _sitemapIndexGenerator;

    //--this is a bogus interface defined in this example to simulate something you might use to get a list of URls from your CMS or something like that
    private readonly IWebsiteUrlRetriever _websiteUrlRetriever;

    //--an IoC/Dependency injection framework should inject this in
    public SitemapGenerationWithSitemapIndexExample(
        ISitemapGenerator sitemapGenerator, 
        ISitemapIndexGenerator sitemapIndexGenerator, 
        IWebsiteUrlRetriever websiteUrlRetriever)
    {
        _sitemapGenerator = sitemapGenerator;
        _sitemapIndexGenerator = sitemapIndexGenerator;
        _websiteUrlRetriever = websiteUrlRetriever;
    }

    //--this is an example showing how you might take a large list of URLs of different kinds of resources and build both a bunch of sitemaps (depending on
    //  how many URls you have) as well as a sitemap index file to go with it
    public void GenerateSitemapsForMyEntireWebsite()
    {
        //--imagine you have an interface that can return a list of URLs for a resource that you consider to be high priority -- for example, the product detail pages (PDPs)
        //  of your website
        var productPageUrlStrings = _websiteUrlRetriever.GetHighPriorityProductPageUrls();

        //--build a list of X.Web.Sitemap.Url objects and determine what is the appropriate ChangeFrequency, TimeStamp (aka "LastMod" or date that the resource last had changes),
        //  and a priority for the page. If you can build in some logic to prioritize your pages then you are more sophisticated than most! :)
        var allUrls = productPageUrlStrings.Select(url => new Url
        {
            //--assign the location of the HTTP request -- e.g.: https://www.somesite.com/some-resource
            Location = url,
            //--let's instruct crawlers to crawl these pages monthly since the content doesn't change that much
            ChangeFrequency = ChangeFrequency.Monthly,
            //--in this case we don't know when the page was last modified so we wouldn't really set this. Only assigning here to demonstrate that the property exists.
            //  if your system is smart enough to know when a page was last modified then that is the best case scenario
            TimeStamp = DateTime.UtcNow,
            //--set this to between 0 and 1. This should only be used as a relative ranking of other pages in your site so that search engines know which result to prioritize
            //  in SERPS if multiple pages look pertinent from your site. Since product pages are really important to us, we'll make them a .9
            Priority = .9
        }).ToList();

        var miscellaneousLowPriorityUrlStrings = _websiteUrlRetriever.GetMiscellaneousLowPriorityUrls();
        var miscellaneousLowPriorityUrls = miscellaneousLowPriorityUrlStrings.Select(url => new Url
        {
            Location = url,
            //--let's instruct crawlers to crawl these pages yearly since the content almost never changes
            ChangeFrequency = ChangeFrequency.Yearly,
            //--let's pretend this content was changed a year ago
            TimeStamp = DateTime.UtcNow.AddYears(-1),
            //--these pages are super low priority
            Priority = .1
        }).ToList();

        //--combine the urls into one big list. These could of course be kept separate and two different sitemap index files could be generated if we wanted
        allUrls.AddRange(miscellaneousLowPriorityUrls);

        //--pick a place where you would like to write the sitemap files (existing files in that folder will get overwritten by new ones)
        var targetSitemapDirectory = new DirectoryInfo("/var/www/mywebsite.com/sitemaps");

        //--generate one or more sitemaps (depending on the number of URLs) in the designated location.
        var fileInfoForGeneratedSitemaps = _sitemapGenerator.GenerateSitemaps(allUrls, targetSitemapDirectory);

        var sitemapInfos = new List<SitemapInfo>();
        var dateSitemapWasUpdated = DateTime.UtcNow.Date;
        
        foreach (var fileInfo in fileInfoForGeneratedSitemaps)
        {
            //--it's up to you to figure out what the URI is to the sitemap you wrote to the file system. In this case we are assuming that the directory above
            //  has files exposed via the /sitemaps/ subfolder of www.mywebsite.com
            var uriToSitemap = new Uri($"https://www.mywebsite.com/sitemaps/{fileInfo.Name}");
           
            sitemapInfos.Add(new SitemapInfo(uriToSitemap, dateSitemapWasUpdated));
        }

        //--now generate the sitemap index file which has a reference to all of the sitemaps that were generated. 
        _sitemapIndexGenerator.GenerateSitemapIndex(sitemapInfos, targetSitemapDirectory, "sitemap-index.xml");

        //-- After this runs you'll want to make sure your robots.txt has a reference to the sitemap index (at the bottom of robots.txt) like this: 
        //  "Sitemap: https://www.mywebsite.com/sitemaps/sitemap-index.xml"
        //  You could do this manually (since this may never change) or if you are ultra-fancy, you could dynamically update your robots.txt with the names of the sitemap index
        //  file(s) you generated

    }


    //--some bogus interface that is meant to simulate pulling urls from your CMS/website
    public interface IWebsiteUrlRetriever
    {
        List<string> GetHighPriorityProductPageUrls();
        List<string> GetMiscellaneousLowPriorityUrls();
    }
}
```

---

## Contributing
Contributions to the `X.Web.Sitemap` library are welcome. Please ensure to follow the contributing guidelines specified in the repository for submitting issues, feature requests, or pull requests.

## License
The `X.Web.Sitemap` library is released under [MIT license](LICENSE.md).
