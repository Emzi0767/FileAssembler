using System;
using System.IO;
using System.Threading.Tasks;

namespace Emzi0767.FileAssembler
{
    public static class Extensions
    {
        public static string ToByteString(this byte[] buff)
            => BitConverter.ToString(buff).ToLowerInvariant().Replace("-", "");
    }
}
