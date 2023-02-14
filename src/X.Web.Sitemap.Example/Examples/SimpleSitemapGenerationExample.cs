namespace X.Web.Sitemap.Example.Examples;

public class SimpleSitemapGenerationExample : IExample
{
    public void Run()
    {
        // Pick a place where you would like to write the sitemap files in that folder will get overwritten by new ones
        var directory = Path.Combine(Path.GetTempPath(), "XWebsiteExample");

        var urlGenerator = new UrlGenerator();

        // Get list of website urls
        var allUrls = urlGenerator.GetUrls("mywebsite.com", false);

        var sitemap = new Sitemap(allUrls);

        sitemap.SaveToDirectory(directory);
        
        Console.WriteLine($"Sitemap stored at: `{directory}`");
    }

}