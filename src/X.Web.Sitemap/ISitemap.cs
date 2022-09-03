using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
public interface ISitemap : IList<Url>
{
    bool Save(string path);

    Task<bool> SaveAsync(string path);
        
    [Obsolete("This method will be removed in future version. Use SitemapGenerator instead")]
    bool SaveToDirectory(string targetSitemapDirectory);
        
    string ToXml();
}