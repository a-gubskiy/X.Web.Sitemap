using System.Collections.Immutable;

namespace X.Web.Sitemap.Example;

public class UrlGenerator
{
    public IReadOnlyCollection<Url> GetUrls(string domain, bool addImages, int? maxUrlCount = null)
    {
        var productPageUrlStrings = GetHighPriorityProductPageUrls(domain);

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

        if (addImages)
        {
            var images = GetUrlWithImages(domain);

            var urlsWithImages = images.Select(x =>
            {
                return new Url
                {
                    Location = x.url,
                    ChangeFrequency = ChangeFrequency.Daily,
                    TimeStamp = DateTime.UtcNow.AddMonths(-1),
                    Priority = .5,
                    Images = new List<Image>
                    {
                        new Image { Location = x.img1 },
                        new Image { Location = x.img2 },
                    }
                };
            }).ToList();


            allUrls.AddRange(urlsWithImages);

        }

        //randomize urls
        var result = allUrls.OrderBy(o => Guid.NewGuid()).ToImmutableArray();

        if (maxUrlCount.HasValue)
        {
            result = result.Take(maxUrlCount.Value).ToImmutableArray();
        }
        
        return result;
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
    
    private IReadOnlyCollection<(string url, string img1, string img2)> GetUrlWithImages(string domain)
    {
        var result = new List<(string, string, string)>();
        
        for (int i = 0; i < 10000; i++)
        {
            result.Add((
                $"https://{domain}/page-with-images/{i}.html",
                $"https://{domain}/files/photo{i}.jpg",
                $"https://{domain}/files/photo_{i}_small.jpg"
            ));
        }

        return result;
    }
}