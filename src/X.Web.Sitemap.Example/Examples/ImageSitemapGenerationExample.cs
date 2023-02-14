namespace X.Web.Sitemap.Example.Examples;

public class ImageSitemapGenerationExample : IExample
{
    public void Run()
    {
        // Pick a place where you would like to write the sitemap files in that folder will get overwritten by new ones
        //var directory = Path.Combine(Path.GetTempPath(), "XWebsiteExample");
        var directory = "/Users/andrew/Downloads/";

        var urlGenerator = new UrlGenerator();

        // Get list of website urls
        var allUrls = urlGenerator.GetUrls("mywebsitewithimages.com", true, 100);

        var sitemap = new Sitemap(allUrls);

        sitemap.SaveToDirectory(directory);
        
        Console.WriteLine($"Sitemap stored at: `{directory}`");
    }
}