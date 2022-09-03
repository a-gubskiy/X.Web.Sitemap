namespace X.Web.Sitemap.Example;

public class SimpleSitemapGenerations : IExample
{
    public void Run()
    {
        // Pick a place where you would like to write the sitemap files in that folder will get overwritten by new ones
        var directory = Path.Combine(Path.GetTempPath(), "XWebsiteExample");

        // Pick a place where sitemaps will be accessible from internet
        var sitemapRootUrl = "https://www.mywebsite.com/sitemaps/";

        var urlGenerator = new UrlGenerator();

        // Get list of website urls
        var allUrls = urlGenerator.GetUrls("mywebsite.com");

        var sitemap = new Sitemap();
        sitemap.AddRange(allUrls);

        sitemap.SaveToDirectory(directory);
    }

}