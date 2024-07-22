# X.Web.Sitemap

[![NuGet Version](http://img.shields.io/nuget/v/X.Web.Sitemap.svg?style=flat)](https://www.nuget.org/packages/X.Web.Sitemap/)
[![Twitter URL](https://img.shields.io/twitter/url/https/twitter.com/andrew_gubskiy.svg?style=social&label=Follow%20me!)](https://twitter.com/intent/user?screen_name=andrew_gubskiy)


X.Web.Sitemap is a library for generating and managing sitemaps in .NET applications.

## Usage example

Below is an example of basic usage in a non-testable manner

```cs
    class Program
    {    
        static void Main(string[] args)
        {
            var sitemap = new Sitemap();

            sitemap.Add(new Url
                {
                    ChangeFrequency = ChangeFrequency.Hourly,
                    Location = "http://www.example.com",
                    Priority = 0.8,
                    TimeStamp = DateTime.Now
                });

            sitemap.Add(CreateUrl("http://www.example.com/link1"));
            sitemap.Add(CreateUrl("http://www.example.com/link2"));
            sitemap.Add(CreateUrl("http://www.example.com/link3"));
            sitemap.Add(CreateUrl("http://www.example.com/link4"));
            sitemap.Add(CreateUrl("http://www.example.com/link5"));


            //Save sitemap structure to file
            sitemap.Save(@"d:\www\example.com\sitemap.xml");

            //Split a large list into pieces and store in a directory
            sitemap.SaveToDirectory(@"d:\www\example.com\sitemaps");

            //Get xml-content of file
            Console.Write(sitemap.ToXml());

            Console.ReadKey();
        }

        private static Url CreateUrl(string url)
        {
            return new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = url,
                    Priority = 0.5,
                    TimeStamp = DateTime.Now
                };
        }
    }
```

Below is a more comprehensive example that demonstrates how to create many sitemaps and how to add them to a sitemap index file in a unit-testable fashion.

```cs
	public class SitemapGenerationWithSitemapIndexExample
    {
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly ISitemapIndexGenerator _sitemapIndexGenerator;

        //--this is a bogus interface defined in this example to simulate something you might use to get a list of URls from your CMS or something like that
        private readonly IWebsiteUrlRetriever _websiteUrlRetriever;

        //--and IoC/Dependency injection framework should inject this in
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
            //  and the a priority for the page. If you can build in some logic to prioritize your pages then you are more sophisticated than most! :)
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

            //--combine the urls into one big list. These could of course bet kept seperate and two different sitemap index files could be generated if we wanted
            allUrls.AddRange(miscellaneousLowPriorityUrls);

            //--pick a place where you would like to write the sitemap files in that folder will get overwritten by new ones
            var targetSitemapDirectory = new DirectoryInfo("\\SomeServer\\some_awesome_file_Share\\sitemaps\\");

            //--generate one or more sitemaps (depending on the number of URLs) in the designated location.
            var fileInfoForGeneratedSitemaps = _sitemapGenerator.GenerateSitemaps(allUrls, targetSitemapDirectory);

            var sitemapInfos = new List<SitemapInfo>();
            var dateSitemapWasUpdated = DateTime.UtcNow.Date;
            
            foreach (var fileInfo in fileInfoForGeneratedSitemaps)
            {
                //--it's up to you to figure out what the URI is to the sitemap you wrote to the file sytsem. In this case we are assuming that the directory above
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
