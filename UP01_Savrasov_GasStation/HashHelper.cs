using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace UP01_Savrasov_GasStation
{
    public static class HashHelper
    {
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }
    }
}