###X.Web.Sitemap [![Build status](https://ci.appveyor.com/api/projects/status?id=m7hb7ukxevvarmvd)](https://ci.appveyor.com/project/X.Web.Sitemap)


Simple sitemap generator for .NET
You can download it from Nuget.org at http://nuget.org/packages/xsitemap/


Sample of use:


    class Program
    {    
        static void Main(string[] args)
        {
            var sitemap = new Sitemap();

            sitemap.Add(new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = "http://www.example.com",
                    Priority = 0.5,
                    TimeStamp = DateTime.Now
                });

            sitemap.Add(CreateUrl("http://www.example.com/link1"));
            sitemap.Add(CreateUrl("http://www.example.com/link2"));
            sitemap.Add(CreateUrl("http://www.example.com/link3"));
            sitemap.Add(CreateUrl("http://www.example.com/link4"));
            sitemap.Add(CreateUrl("http://www.example.com/link5"));


            //Save sitemap structure to file
            sitemap.Save(@"d:\www\example.com\sitemap.xml");

            //Split a large list into pieces and store in a directory
            sitemap.SaveToDirectory(@"d:\www\example.com\sitemaps");

            //Get xml-content of file
            Console.Write(sitemap.ToXml());

            Console.ReadKey();
        }

        private static Url CreateUrl(string url)
        {
            return new Url
                {
                    ChangeFrequency = ChangeFrequency.Daily,
                    Location = url,
                    Priority = 0.5,
                    TimeStamp = DateTime.Now
                };
        }
    }

