using System;

namespace forumx_server.Model
{
    public class User
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public byte[] TotpToken { get; set; }
        public bool ActivatedUser { get; set; }
        public byte[] PasswordHash { get; set; }
        public string Password { get; set; }
    }
}