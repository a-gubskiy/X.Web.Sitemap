using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap.Example.Examples;

public class ImageSitemapGenerationExample : IExample
{
    public void Run()
    {
        // Pick a place where you would like to write the sitemap files in that folder will get overwritten by new ones
        //var directory = Path.Combine(Path.GetTempPath(), "XWebsiteExample");
        var directory = "/Users/andrew/Downloads/";

        // Get list of website urls
        IReadOnlyCollection<Url> allUrls = //urlGenerator.GetUrls("mywebsitewithimages.com", true, 100);
            new[]
            {
                new Url
                {
                    Images = new List<Image>
                    {
                        new Image { Location = "http://exmaple.com/1.jpg" },
                        new Image { Location = "http://exmaple.com/2.jpg" },
                    },
                    Location = "http://exmaple.com",
                    TimeStamp = DateTime.Today,
                    Priority = 1.0,
                    ChangeFrequency = ChangeFrequency.Daily
                },
                new Url
                {
                    Images = new List<Image>
                    {
                        new Image { Location = "http://exmaple.com/3.jpg" },
                        new Image { Location = "http://exmaple.com/4.jpg" },
                        new Image { Location = "http://exmaple.com/5.jpg" },
                    },
                    Location = "http://exmaple.com/page/1",
                    TimeStamp = DateTime.Today,
                    Priority = 1.0,
                    ChangeFrequency = ChangeFrequency.Daily
                }
            };

        var sitemap = new Sitemap(allUrls);
        sitemap.SaveToDirectory(directory);

        var xml = sitemap.ToXml();
        
        Console.WriteLine($"Sitemap:");
        Console.WriteLine(xml);
    }
}