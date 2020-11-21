using System.Net;

namespace forumx_server.Captcha
{
    public interface ICaptcha
    {
        public bool VerifyCaptcha(string captchaResponse, IPAddress ipAddress, string action);
    }
}