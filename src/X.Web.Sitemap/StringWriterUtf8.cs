using System.IO;
using System.Text;

namespace X.Web.Sitemap;

/// <summary>
/// Subclass the StringWriter class and override the default encoding.  
/// This allows us to produce XML encoded as UTF-8. 
/// </summary>
public class StringWriterUtf8 : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}