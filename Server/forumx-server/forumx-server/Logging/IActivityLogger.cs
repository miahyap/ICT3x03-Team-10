using System.Net;
using forumx_server.Model;

namespace forumx_server.Logging
{
    public interface IActivityLogger
    {
        public void LogLogin(IPAddress ipAddress, User user);
        public void LogRegister(IPAddress ipAddress, User user);
        public void LogResetPassword(IPAddress ipAddress, User user);
        public void LogChangePassword(IPAddress ipAddress, User user);
        public void LogTerminateSession(IPAddress ipAddress, User user);
        public void LogNewPost(IPAddress ipAddress, User user, Post post);
        public void LogEditPost(IPAddress ipAddress, User user, Post post);
        public void LogDeletePost(IPAddress ipAddress, User user, Post post);
        public void LogNewComment(IPAddress ipAddress, User user, Comment comment);
        public void LogEditComment(IPAddress ipAddress, User user, Comment comment);
        public void LogDeleteComment(IPAddress ipAddress, User user, Comment comment);
    }
}