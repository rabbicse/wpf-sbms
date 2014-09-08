using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Utility.Crypto
{
    public class CryptoUtils
    {
        public static string CreateSha1Hash(byte[] bytes)
        {
            SHA1 hash = new SHA1CryptoServiceProvider();
            byte[] hashBytes = new byte[bytes.Length];
            byte[] hashResult = null;

            System.Buffer.BlockCopy(bytes, 0, hashBytes, 0, bytes.Length);

            hashResult = hash.ComputeHash(hashBytes);
            return Convert.ToBase64String(hashResult);
        }
    }
}
