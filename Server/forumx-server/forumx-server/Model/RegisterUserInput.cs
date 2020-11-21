namespace forumx_server.Model
{
    public class RegisterUserInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string Captcha { get; set; }
    }
}