using System.IO;
using JetBrains.Annotations;

namespace X.Web.Sitemap
{
    [PublicAPI]
    internal interface ISerializedXmlSaver<in T>
    {
        FileInfo SerializeAndSave(T objectToSerialize, DirectoryInfo targetDirectory, string targetFileName);
    }
}