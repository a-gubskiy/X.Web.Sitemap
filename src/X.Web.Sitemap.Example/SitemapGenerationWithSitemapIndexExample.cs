namespace X.Web.Sitemap.Example;

/// <summary>
/// This is an example showing how you might take a large list of URLs of different kinds of resources and build
/// both a bunch of sitemaps (depending on how many URls you have) as well as a sitemap index file to go with it
/// </summary>
public class SitemapGenerationWithSitemapIndexExample : IExample
{
    public void Run()
    { 
        // Pick a place where you would like to write the sitemap files in that folder will get overwritten by new ones
        var path = Path.Combine(Path.GetTempPath(), "XWebsiteExample");
        var targetSitemapDirectory = new DirectoryInfo(path);
        
        // Pick a place where sitemaps will be accessible from internet
        var sitemapRootUrl = "https://www.mywebsite.com/sitemaps/";


        var sitemapGenerator = new SitemapGenerator();
        var sitemapIndexGenerator = new SitemapIndexGenerator();
        
        var productPageUrlStrings = GetHighPriorityProductPageUrls();

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

        var miscellaneousLowPriorityUrlStrings = GetMiscellaneousLowPriorityUrls();
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

        

        //--generate one or more sitemaps (depending on the number of URLs) in the designated location.
        var fileInfoForGeneratedSitemaps = sitemapGenerator.GenerateSitemaps(allUrls, targetSitemapDirectory);

        var sitemapInfos = new List<SitemapInfo>();
        var dateSitemapWasUpdated = DateTime.UtcNow.Date;
            
        foreach (var fileInfo in fileInfoForGeneratedSitemaps)
        {
            //--it's up to you to figure out what the URI is to the sitemap you wrote to the file sytsem. In this case we are assuming that the directory above
            //  has files exposed via the /sitemaps/ subfolder of www.mywebsite.com
            
            var uriToSitemap = new Uri($"{sitemapRootUrl}{fileInfo.Name}");
               
            sitemapInfos.Add(new SitemapInfo(uriToSitemap, dateSitemapWasUpdated));
        }

        //--now generate the sitemap index file which has a reference to all of the sitemaps that were generated. 
        sitemapIndexGenerator.GenerateSitemapIndex(sitemapInfos, targetSitemapDirectory, "sitemap-index.xml");

        //-- After this runs you'll want to make sure your robots.txt has a reference to the sitemap index (at the bottom of robots.txt) like this: 
        //  "Sitemap: https://www.mywebsite.com/sitemaps/sitemap-index.xml"
        //  You could do this manually (since this may never change) or if you are ultra-fancy, you could dynamically update your robots.txt with the names of the sitemap index
        //  file(s) you generated
    }

    private IReadOnlyCollection<string> GetMiscellaneousLowPriorityUrls()
    {
        var result = new List<string>();
        
        for (int i = 0; i < 40000; i++)
        {
            result.Add($"https://example.com/page/{i}.html");
        }

        return result;
    }

    private IReadOnlyCollection<string> GetHighPriorityProductPageUrls()
    {
        var result = new List<string>();
        
        for (int i = 0; i < 10000; i++)
        {
            result.Add($"https://example.com/priority-page/{i}.html");
        }

        return result;
    }
}