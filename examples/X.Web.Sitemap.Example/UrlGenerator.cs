namespace X.Web.Sitemap.Example;

public class UrlGenerator
{
    public List<Url> GetUrls(string domain)
    {
        var productPageUrlStrings = GetHighPriorityProductPageUrls(domain);

        //--build a list of X.Web.Sitemap.Url objects and determine what is the appropriate ChangeFrequency, TimeStamp (aka "LastMod" or date that the resource last had changes),
        //  and the priority for the page. If you can build in some logic to prioritize your pages then you are more sophisticated than most! :)
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

        var miscellaneousLowPriorityUrlStrings = GetMiscellaneousLowPriorityUrls(domain);
        
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

        return allUrls;
    }

    private IReadOnlyCollection<string> GetMiscellaneousLowPriorityUrls(string domain)
    {
        var result = new List<string>();
        
        for (int i = 0; i < 40000; i++)
        {
            result.Add($"https://{domain}/page/{i}.html");
        }

        return result;
    }

    private IReadOnlyCollection<string> GetHighPriorityProductPageUrls(string domain)
    {
        var result = new List<string>();
        
        for (int i = 0; i < 10000; i++)
        {
            result.Add($"https://{domain}/priority-page/{i}.html");
        }

        return result;
    }
}