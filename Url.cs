using System;

namespace X.Web.Sitemap
{
    [Serializable]
    public class Url
    {
        public String Location { get; set; }
        public DateTime TimeStamp { get; set; }
        public ChangeFrequency ChangeFrequency { get; set; }
        public double Priority { get; set; }

        public Url()
        {
        }

        public static Url CreateUrl(string location)
        {
            return CreateUrl(location, DateTime.Now);
        }

        public static Url CreateUrl(string location, DateTime timeStamp)
        {
            return new Url
                       {
                           Location = location,
                           ChangeFrequency = ChangeFrequency.Daily,
                           Priority = 0.5d,
                           TimeStamp = timeStamp,
                       };
        }
    }
}
