using System.Xml;
using Xunit;
using X.Web.Sitemap.Extensions;

namespace X.Web.Sitemap.Tests.UnitTests
{
    public class XmlDocumentExtensionTests
    {
        [Fact]
        public void ToXml_ReturnsStringRepresentation()
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("root");
            doc.AppendChild(root);
            var child = doc.CreateElement("child");
            child.InnerText = "hello";
            root.AppendChild(child);

            var xml = doc.ToXml();

            Assert.Contains("<root>", xml);
            Assert.Contains("hello", xml);
        }
    }
}

