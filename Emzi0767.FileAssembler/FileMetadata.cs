using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Emzi0767.FileAssembler
{
    [XmlRoot(ElementName = "file", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
    public sealed class FileMetadata
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Xmlns { get; } = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("fa", "https://data.emzi0767.com/xml/fileassembler") });

        [XmlAttribute(AttributeName = "name", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "length", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        public long Length { get; set; }
        
        [XmlElement(ElementName = "chunk-size", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        public long ChunkSize { get; set; }

        [XmlElement(ElementName = "sha256", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        public string Sha256 { get; set; }

        [XmlArray(ElementName = "chunks", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        [XmlArrayItem(ElementName = "file-chunk", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        public ChunkMetadata[] Chunks { get; set; }

        public FileMetadata() { }
        public FileMetadata(FileInfo file, long chunkSize, byte[] sha256, ChunkMetadata[] chunks)
        {
            this.Name = file.Name;
            this.Length = file.Length;
            this.ChunkSize = chunkSize;
            this.Sha256 = sha256.ToByteString();
            this.Chunks = chunks;
        }
    }

    public class ChunkMetadata
    {
        [XmlElement(ElementName = "file-name", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        public string Filename { get; set; }

        [XmlElement(ElementName = "sha256", Namespace = "https://data.emzi0767.com/xml/fileassembler")]
        public string Sha256 { get; set; }

        public ChunkMetadata() { }
        public ChunkMetadata(FileInfo file, byte[] sha256)
        {
            this.Filename = file.Name;
            this.Sha256 = sha256.ToByteString();
        }
    }
}
