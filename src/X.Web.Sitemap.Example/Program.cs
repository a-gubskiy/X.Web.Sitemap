using X.Web.Sitemap;
using X.Web.Sitemap.Example;
using X.Web.Sitemap.Example.Examples;

Console.WriteLine("OK");

Sitemap.DefaultMaxNumberOfUrlsPerSitemap = 50000;

IExample example1 = new SitemapGenerationWithSitemapIndexExample();
example1.Run();

IExample example2 = new SimpleSitemapGenerationExample();
example2.Run();

IExample example3 = new ImageSitemapGenerationExample();
example3.Run();