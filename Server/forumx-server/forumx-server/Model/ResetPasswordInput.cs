using System.ComponentModel.DataAnnotations;

namespace forumx_server.Model
{
    public class ResetPasswordInput
    {
        [Required] public string Token { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Captcha { get; set; }
    }
}