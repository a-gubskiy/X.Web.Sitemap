using System.IO;

namespace X.Web.Sitemap
{
    public interface ISerializedXmlSaver<in T>
    {
        void SerializeAndSave(T objectToSerialize, DirectoryInfo targetDirectory, string targetFileName);
    }
}