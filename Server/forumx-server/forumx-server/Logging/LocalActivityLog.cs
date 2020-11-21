using System.Net;
using forumx_server.Database;
using forumx_server.Model;

namespace forumx_server.Logging
{
    public class LocalActivityLog : IActivityLogger
    {
        private readonly IDatabase _database;

        public LocalActivityLog(IDatabase database)
        {
            _database = database;
        }

        public void LogEditComment(IPAddress ipAddress, User user, Comment comment)
        {
            var activity = "Comment edited";
            LogActivity(ipAddress, activity, user);
        }

        public void LogDeleteComment(IPAddress ipAddress, User user, Comment comment)
        {
            var activity = "Comment deleted";
            LogActivity(ipAddress, activity, user);
        }

        public void LogEditPost(IPAddress ipAddress, User user, Post post)
        {
            var activity = "Post edited";
            LogActivity(ipAddress, activity, user);
        }

        public void LogDeletePost(IPAddress ipAddress, User user, Post post)
        {
            var activity = "Post deleted";
            LogActivity(ipAddress, activity, user);
        }

        public void LogLogin(IPAddress ipAddress, User user)
        {
            var activity = "Account Login";
            LogActivity(ipAddress, activity, user);
        }

        public void LogNewComment(IPAddress ipAddress, User user, Comment comment)
        {
            var activity = "New comment posted";
            LogActivity(ipAddress, activity, user);
        }

        public void LogTerminateSession(IPAddress ipAddress, User user)
        {
            var activity = "Terminated sessions";
            LogActivity(ipAddress, activity, user);
        }

        public void LogNewPost(IPAddress ipAddress, User user, Post post)
        {
            var activity = "New post created";
            LogActivity(ipAddress, activity, user);
        }

        public void LogRegister(IPAddress ipAddress, User user)
        {
            var activity = "Account created";
            LogActivity(ipAddress, activity, user);
        }

        public void LogResetPassword(IPAddress ipAddress, User user)
        {
            var activity = "Password reset";
            LogActivity(ipAddress, activity, user);
        }

        public void LogChangePassword(IPAddress ipAddress, User user)
        {
            var activity = "Password changed";
            LogActivity(ipAddress, activity, user);
        }

        public void LogActivity(IPAddress ipAddress, string activity, User user)
        {
            _database.LogActivity(ipAddress, activity, user);
        }
    }
}