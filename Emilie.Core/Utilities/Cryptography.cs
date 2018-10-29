using System.Security.Cryptography;
using System.Text;

namespace Emilie.Core.Utilities
{
    public static class Cryptography
    {
        public static string GetMD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return EncodeToString(hash);
            }
        }

        private static string EncodeToString(byte[] hash)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }

        public static ulong GetDJB2Hash(string input)
        {
            ulong hash = 5381;

            for (int c = 0; c < input.Length; c++)
                hash = ((hash << 5) + hash) + input[c]; /* hash * 33 + c */

            return hash;
        }


    }
}
