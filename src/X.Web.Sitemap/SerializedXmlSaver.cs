using System;
using System.IO;
using System.Xml.Serialization;

namespace X.Web.Sitemap;

internal interface ISerializedXmlSaver<in T>
{
    FileInfo SerializeAndSave(T objectToSerialize, DirectoryInfo targetDirectory, string targetFileName);
}

internal class SerializedXmlSaver<T> : ISerializedXmlSaver<T>
{
    private readonly IFileSystemWrapper _fileSystemWrapper;

    public SerializedXmlSaver(IFileSystemWrapper fileSystemWrapper)
    {
        _fileSystemWrapper = fileSystemWrapper;
    }

    public FileInfo SerializeAndSave(T objectToSerialize, DirectoryInfo targetDirectory, string targetFileName)
    {
        if (objectToSerialize == null)
        {
            throw new ArgumentNullException(nameof(objectToSerialize));
        }

        var xmlSerializer = new XmlSerializer(typeof(T));
            
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("image", "http://www.google.com/schemas/sitemap-image/1.1");

        using (var textWriter = new StringWriterUtf8())
        {
            xmlSerializer.Serialize(textWriter, objectToSerialize, namespaces);
            var xmlString = textWriter.ToString();
            var path = Path.Combine(targetDirectory.FullName, targetFileName);
                
            return _fileSystemWrapper.WriteFile(xmlString, path);
        }
    }
}