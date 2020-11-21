using System;

namespace forumx_server.Model
{
    public class Comment
    {
        public string Uuid { get; set; }
        public string Post { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
        public bool Edited { get; set; }
        public DateTime PostedTime { get; set; }
        public string Captcha { get; set; }
    }
}