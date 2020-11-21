using System;
using System.Security.Cryptography;

namespace forumx_server.Helper
{
    public class SecureGuid
    {
        public static Guid CreateSecureRfc4122Guid()
        {
            using var cryptoProvider = new RNGCryptoServiceProvider();

            // byte indices
            var versionByteIndex = BitConverter.IsLittleEndian ? 7 : 6;
            const int variantByteIndex = 8;

            // version mask & shift for `Version 4`
            const int versionMask = 0x0F;
            const int versionShift = 0x40;

            // variant mask & shift for `RFC 4122`
            const int variantMask = 0x3F;
            const int variantShift = 0x80;

            // get bytes of cryptographically-strong random values
            var bytes = new byte[16];
            cryptoProvider.GetBytes(bytes);

            // Set version bits -- 6th or 7th byte according to Endianness, big or little Endian respectively
            bytes[versionByteIndex] = (byte) ((bytes[versionByteIndex] & versionMask) | versionShift);

            // Set variant bits -- 9th byte
            bytes[variantByteIndex] = (byte) ((bytes[variantByteIndex] & variantMask) | variantShift);

            // Initialize Guid from the modified random bytes
            return new Guid(bytes);
        }

        public static bool VerifyGuid(string input, out Guid guidOutput)
        {
            return Guid.TryParse(input, out guidOutput);
        }
    }
}