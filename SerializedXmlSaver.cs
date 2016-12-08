using System;
using System.IO;
using System.Xml.Serialization;

namespace X.Web.Sitemap
{
    public class SerializedXmlSaver<T> : ISerializedXmlSaver<T>
    {
        private readonly IFileSystemWrapper _fileSystemWrapper;

        public SerializedXmlSaver(IFileSystemWrapper fileSystemWrapper)
        {
            _fileSystemWrapper = fileSystemWrapper;
        }

        public void SerializeAndSave(T objectToSerialize, DirectoryInfo targetDirectory, string targetFileName)
        {
            ValidateArgumentNotNull(objectToSerialize);

            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var textWriter = new StringWriterUtf8())
            {
                xmlSerializer.Serialize(textWriter, objectToSerialize);
                var xmlString = textWriter.ToString();
                _fileSystemWrapper.WriteFile(xmlString, targetDirectory, targetFileName);
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