using System.Text;
using Xunit;
using X.Web.Sitemap;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class StringWriterUtf8Tests
    {
        [Fact]
        public void Encoding_IsUtf8()
        {
            var sw = new StringWriterUtf8();
            Assert.Equal(Encoding.UTF8, sw.Encoding);
        }
    }
}

