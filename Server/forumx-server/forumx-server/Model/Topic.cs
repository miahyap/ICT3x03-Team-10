using System.Collections.Generic;

namespace forumx_server.Model
{
    public class Topic
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; }
    }
}