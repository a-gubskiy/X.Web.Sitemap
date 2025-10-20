using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using X.Web.Sitemap;
using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class PublicSaveFailureTests
    {
        [Fact]
        public void PublicSave_ReturnsFalse_OnInvalidPath()
        {
            var sitemap = new Sitemap { Url.CreateUrl("http://example.com/ps1") };
            // filename only should cause EnsureDirectoryCreated to throw
            var result = ((ISitemap)sitemap).Save("just-a-file.xml");

            Assert.False(result);
        }

        [Fact]
        public async Task PublicSaveAsync_ReturnsFalse_OnInvalidPath()
        {
            var sitemap = new Sitemap { Url.CreateUrl("http://example.com/ps2") };

            var result = await ((ISitemap)sitemap).SaveAsync("just-another-file.xml");

            Assert.False(result);
        }
    }
}

