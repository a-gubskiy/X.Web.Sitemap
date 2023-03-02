namespace X.Web.Sitemap.Example.Examples;

/// <summary>
/// This is an example showing how you might take a large list of URLs of different kinds of resources and build
/// both a bunch of sitemaps (depending on how many URls you have) as well as a sitemap index file to go with it
/// </summary>
public class SitemapGenerationWithSitemapIndexExample : IExample
{
    public void Run()
    { 
        // Pick a place where you would like to write the sitemap files in that folder will get overwritten by new ones
        var targetSitemapDirectory = Path.Combine(Path.GetTempPath(), "XWebsiteExample");
        
        // Pick a place where sitemaps will be accessible from internet
        var sitemapRootUrl = "https://www.mywebsite.com/sitemaps/";

        var sitemapGenerator = new SitemapGenerator();
        var sitemapIndexGenerator = new SitemapIndexGenerator();
        var urlGenerator = new UrlGenerator();

        // Get list of website urls
        var allUrls = urlGenerator.GetUrls("mywebsite.com"); 
        

        // generate one or more sitemaps (depending on the number of URLs) in the designated location.
        var fileInfoForGeneratedSitemaps = sitemapGenerator.GenerateSitemaps(allUrls, targetSitemapDirectory);

        var sitemapInfos = new List<SitemapInfo>();
        var dateSitemapWasUpdated = DateTime.UtcNow.Date;
            
        foreach (var fileInfo in fileInfoForGeneratedSitemaps)
        {
            // It's up to you to figure out what the URI is to the sitemap you wrote to the file sytsem.
            // In this case we are assuming that the directory above has files exposed
            // via the /sitemaps/ subfolder of www.mywebsite.com
            
            var uriToSitemap = new Uri($"{sitemapRootUrl}{fileInfo.Name}");
               
            sitemapInfos.Add(new SitemapInfo(uriToSitemap, dateSitemapWasUpdated));
        }

        // Now generate the sitemap index file which has a reference to all of the sitemaps that were generated. 
        sitemapIndexGenerator.GenerateSitemapIndex(sitemapInfos, targetSitemapDirectory, "sitemap-index.xml");

        // After this runs you'll want to make sure your robots.txt has a reference to the sitemap index (at the bottom of robots.txt) like this: 
        // "Sitemap: https://www.mywebsite.com/sitemaps/sitemap-index.xml"
        // You could do this manually (since this may never change) or if you are ultra-fancy, you could dynamically update your robots.txt with the names of the sitemap index
        // file(s) you generated
    }
}