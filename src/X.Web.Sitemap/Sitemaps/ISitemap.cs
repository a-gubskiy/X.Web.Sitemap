using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.Sitemap
{
    [PublicAPI]
    public interface ISitemapBase : IEnumerable<Url>
    {
        bool Save(string path);

        Task<bool> SaveAsync(string path);
        
        bool SaveToDirectory(string directory);
        
        string ToXml();
    }

    [PublicAPI]
    public interface ISitemap : IList<Url>, ISitemapBase
    {
    }
}