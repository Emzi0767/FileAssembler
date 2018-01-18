using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Emzi0767.FileAssembler
{
    public sealed class Splitter
    {
        public async Task SplitFileAsync(FileInfo file, int chunkSize, DirectoryInfo output)
        {
            var buff0 = new byte[4 * 1024 * 1024];
            var buff1 = new byte[buff0.Length];
            var cklr = chunkSize;
            FileInfo ccf = null;
            FileStream ccfs = null;
            SHA256 cch = null;
            List<ChunkMetadata> chunks = null;
            FileMetadata meta = null;

            Console.WriteLine("Splitting file");

            using (var fs = file.OpenRead())
            using (var sha256 = SHA256.Create())
            {
                chunks = new List<ChunkMetadata>((int)(fs.Length / chunkSize + 1));

                while (fs.Position < fs.Length)
                {
                    // establish current length
                    var lr = fs.Length - fs.Position;

                    // read the data from the file
                    var br = await fs.ReadAsync(buff0, 0, buff0.Length);
                    var rbr = br;

                    // hash that chunk
                    if (br == buff0.Length && fs.Length > fs.Position)
                        sha256.TransformBlock(buff0, 0, br, buff1, 0);
                    else
                        sha256.TransformFinalBlock(buff0, 0, br);

                    // chunk the buffer, accounting for chunk boundaries
                    while (rbr > 0)
                    {
                        // are we working on a chunk?
                        if (ccf == null)
                        {
                            // we're not, so let's initialize a chunk
                            var ccn = chunks.Count;
                            var ccp = Path.Combine(output.FullName, file.Name);
                            ccp = $"{ccp}.{ccn:000}.pdat";
                            ccf = new FileInfo(ccp);
                            ccfs = ccf.Create();
                            cch = SHA256.Create();
                            cklr = lr > chunkSize ? chunkSize : (int)lr;

                            Console.WriteLine($"Beginning chunk {ccn}");
                        }

                        // are we crossing chunk boundary?
                        if (cklr <= br)
                        {
                            // yes
                            await ccfs.WriteAsync(buff0, br - rbr, cklr);
                            await ccfs.FlushAsync();
                            cch.TransformFinalBlock(buff0, br - rbr, cklr);

                            rbr -= cklr;
                            cklr = 0;
                        }
                        else
                        {
                            // no
                            await ccfs.WriteAsync(buff0, br - rbr, rbr);
                            cch.TransformBlock(buff0, br - rbr, rbr, buff1, 0);

                            cklr -= rbr;
                            rbr = 0;
                        }

                        // are we finished with this chunk?
                        if (cklr == 0)
                        {
                            // we are, so let's compute the metadata and go home
                            chunks.Add(new ChunkMetadata(ccf, cch.Hash));

                            Console.WriteLine($"Written to {ccf.Name}");

                            // clean up the resources
                            ccf = null;
                            ccfs.Dispose();
                            ccfs = null;
                            cch.Dispose();
                            cch = null;
                        }
                    }
                }

                // compute chunk metadata
                Console.WriteLine("Computing metadata");
                meta = new FileMetadata(file, chunkSize, sha256.Hash, chunks.ToArray());
            }

            // compute 
            Console.WriteLine("Writing metadata");
            var mfn = $"{file.Name}.pmeta";
            var mfp = Path.Combine(output.FullName, mfn);
            var mfo = new FileInfo(mfp);
            var xml = new XmlSerializer(typeof(FileMetadata));
            using (var mfs = mfo.Create())
                xml.Serialize(mfs, meta);

            Console.WriteLine("Operation completed");
            Console.WriteLine("Filename:          {0}", file.Name);
            Console.WriteLine("Chunks written to: {0}", output.FullName);
            Console.WriteLine("Chunk count:       {0}", meta.Chunks.Length);
            Console.WriteLine("SHA256:            {0}", meta.Sha256);
        }
    }
}
