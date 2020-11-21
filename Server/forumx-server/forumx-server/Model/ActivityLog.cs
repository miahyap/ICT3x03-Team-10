using System;

namespace forumx_server.Model
{
    public class ActivityLog
    {
        public string Source { get; set; }
        public string Activity { get; set; }
        public DateTime Time { get; set; }
    }
}