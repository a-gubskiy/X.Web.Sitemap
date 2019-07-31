using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Web.Sitemap
{
    public interface ISitemap : IList<Url>
    {
        bool Save(string path);

        Task<bool> SaveAsync(string path);
        
        bool SaveToDirectory(string directory);
        
        string ToXml();
    }
}