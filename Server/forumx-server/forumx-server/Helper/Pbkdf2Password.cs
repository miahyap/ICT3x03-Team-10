using System.Linq;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace forumx_server.Helper
{
    public class Pbkdf2Password
    {
        public static byte[] PasswordToHash(string password)
        {
            var salt = new byte[16];
            var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(salt);
            var hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 10000, 32);
            return salt.Concat(hash).ToArray();
        }

        public static bool CheckPasswordHash(byte[] hashBytes, string password)
        {
            var salt = hashBytes[.. 16];
            var originalHash = hashBytes[16 .. 48];
            var hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, 10000, 32);
            return hash.SequenceEqual(originalHash);
        }
    }
}