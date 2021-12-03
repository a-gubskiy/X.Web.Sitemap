using System;
using System.IO;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace X.Web.Sitemap
{
    [PublicAPI]
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
            ValidateArgumentNotNull(objectToSerialize);

            var xmlSerializer = new XmlSerializer(typeof(T));
            
            using (var textWriter = new StringWriterUtf8())
            {
                xmlSerializer.Serialize(textWriter, objectToSerialize);
                var xmlString = textWriter.ToString();
                var path = Path.Combine(targetDirectory.FullName, targetFileName);
                
                return _fileSystemWrapper.WriteFile(xmlString, path);
            }
        }

        private static void ValidateArgumentNotNull(T objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException(nameof(objectToSerialize));
            }
        }
    }
}