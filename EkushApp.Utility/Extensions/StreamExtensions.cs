using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Utility.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream == null) return null;
            byte[] bytes;
            using (stream)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    stream.CopyTo(memStream);
                    bytes = memStream.ToArray();
                }
            }
            return bytes;
        }
        public static Stream ToStream(this byte[] buffer)
        {
            if (buffer == null) return null;
            return new MemoryStream(buffer);
        }
    }
}
