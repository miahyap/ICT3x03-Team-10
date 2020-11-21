using System;

namespace forumx_server.Model
{
    public class TokenInfo
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public DateTime Expiry { get; set; }
    }
}