using System.Collections.Generic;
using JetBrains.Annotations;

namespace X.Web.Sitemap;

[PublicAPI]
public interface ISitemap : IList<Url>
{
}