using System;
using System.Xml;
using Xunit;
using X.Web.Sitemap;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class SitemapIndexSerializerTests
    {
        [Fact]
        public void Serialize_Null_ThrowsArgumentNullException()
        {
            var serializer = new SitemapIndexSerializer();
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null!));
        }

        [Fact]
        public void Deserialize_Empty_ThrowsArgumentException()
        {
            var serializer = new SitemapIndexSerializer();
            Assert.Throws<ArgumentException>(() => serializer.Deserialize(string.Empty));
        }

        [Fact]
        public void Deserialize_InvalidXml_ThrowsInvalidOperationException()
        {
            var serializer = new SitemapIndexSerializer();
            // malformed xml that won't deserialize into a SitemapIndex
            Assert.Throws<InvalidOperationException>(() => serializer.Deserialize("<notvalid></notvalid>"));
        }
    }
}
