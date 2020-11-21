namespace forumx_server.Model
{
    public class LoginInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Captcha { get; set; }
    }
}