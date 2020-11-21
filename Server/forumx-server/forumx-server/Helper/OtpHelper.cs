using OtpNet;

namespace forumx_server.Helper
{
    public class OtpHelper
    {
        public static bool VerifyOtp(byte[] key, string otpCode)
        {
            var totp = new Totp(key, mode: OtpHashMode.Sha1, totpSize: 6);
            return totp.VerifyTotp(otpCode, out _);
        }

        public static string GenerateTotp(byte[] key)
        {
            var base32Secret = Base32Encoding.ToString(key);
            base32Secret = base32Secret.TrimEnd('=');
            var qrString = $"otpauth://totp/forumx?secret={base32Secret}&digits=6&period=30";
            return qrString;
        }
    }
}