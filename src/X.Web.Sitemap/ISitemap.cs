using System.Collections.Generic;

namespace X.Web.Sitemap
{
    public interface ISitemap : IList<Url>
    {
        bool Save(string path);
        bool SaveToDirectory(string directory);
        string ToXml();
    }
}