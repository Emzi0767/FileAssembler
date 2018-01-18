using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Emzi0767.FileAssembler
{
    public sealed class Unsplitter
    {
        public async Task UnsplitFileAsync(FileInfo file, DirectoryInfo output)
        {
            var buff0 = new byte[4 * 1024 * 1024];
            var buff1 = new byte[buff0.Length];
            FileMetadata meta = null;

            // load the metadata
            Console.WriteLine("Reading metadata");
            var xml = new XmlSerializer(typeof(FileMetadata));
            using (var mfs = file.OpenRead())
                meta = xml.Deserialize(mfs) as FileMetadata;

            Console.WriteLine("Assembling file");
            Console.WriteLine("Filename:          {0}", meta.Name);
            Console.WriteLine("Target directory:  {0}", output.FullName);
            Console.WriteLine("Chunk count:       {0}", meta.Chunks.Length);
            Console.WriteLine("SHA256:            {0}", meta.Sha256);
            
            var ofp = Path.Combine(output.FullName, meta.Name);
            var ofi = new FileInfo(ofp);
            using (var fs = ofi.Create())
            using (var sha256 = SHA256.Create())
            {
                // chunk number
                var ccn = 0;
                
                // enumerate chunks
                foreach (var cc in meta.Chunks)
                {
                    Console.WriteLine($"Current chunk: {ccn}");

                    // get the current chunk file
                    var cfp = Path.Combine(file.Directory.FullName, cc.Filename);
                    var cfi = new FileInfo(cfp);

                    // start reading from the chunk file
                    using (var ccfs = cfi.OpenRead())
                    using (var cch = SHA256.Create())
                    {
                        while (ccfs.Position < ccfs.Length)
                        {
                            // read from the chunk file
                            var br = await ccfs.ReadAsync(buff0, 0, buff0.Length);

                            // hash the buffer
                            if (br == buff0.Length && ccfs.Length > ccfs.Position)
                            {
                                sha256.TransformBlock(buff0, 0, br, buff1, 0);
                                cch.TransformBlock(buff0, 0, br, buff1, 0);
                            }
                            else
                            {
                                if (ccn + 1 == meta.Chunks.Length)
                                    sha256.TransformFinalBlock(buff0, 0, br);
                                else
                                    sha256.TransformBlock(buff0, 0, br, buff1, 0);

                                cch.TransformFinalBlock(buff0, 0, br);
                            }

                            // write to the main file
                            await fs.WriteAsync(buff0, 0, br);
                        }

                        // verify chunk hash
                        var ccsum = cch.Hash.ToByteString();
                        if (ccsum != cc.Sha256)
                            throw new InvalidDataException("Chunk checksum does not match recorded checksum. This is usually an indicator of corrupted chunk data.");
                    }

                    // increment chunk number
                    ccn++;
                }

                // flush the main file buffer
                await fs.FlushAsync();

                // verify file hash
                var cksum = sha256.Hash.ToByteString();
                if (cksum != meta.Sha256)
                    throw new InvalidDataException("File checksum does not match recorded checksum. This is usually an indicator of corrupted chunk data.");
            }

            Console.WriteLine($"File assembled to {ofp}");
        }
    }
}
