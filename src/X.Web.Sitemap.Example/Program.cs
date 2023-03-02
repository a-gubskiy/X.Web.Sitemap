using X.Web.Sitemap.Example;

Console.WriteLine("OK");

IExample example1 = new SitemapGenerationWithSitemapIndexExample();
example1.Run();

IExample example2 = new SimpleSitemapGenerationExample();
example2.Run();

IExample example3 = new ImageSitemapGenerationExample();
example3.Run();