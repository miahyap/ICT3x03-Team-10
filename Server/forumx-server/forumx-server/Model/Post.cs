using System;
using System.Collections.Generic;

namespace forumx_server.Model
{
    public class Post
    {
        public string Uuid { get; set; }
        public string User { get; set; }
        public string Topic { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Edited { get; set; }
        public DateTime PostedTime { get; set; }
        public List<Comment> Comments { get; set; }
        public string Captcha { get; set; }
    }
}