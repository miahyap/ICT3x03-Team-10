using System;
using System.Collections.Generic;
using System.Net;
using forumx_server.Model;

namespace forumx_server.Database
{
    public interface IDatabase
    {
        public void AddUser(User user);
        public User GetUser(string username);
        public User GetUser(Guid userGuid);
        public void ChangePassword(User user);
        public bool CheckEmailExists(string email);
        public List<Topic> GetTopics(string topicUuid);
        public Post GetPostInfo(Guid postGuid);
        public List<Post> GetPostByTopic(Guid topicUuid);
        public List<Comment> GetCommentsByPost(Guid postUuid);
        public bool UpdatePost(Post post, User user);
        public bool DeletePost(Post post, User user);
        public bool CreatePost(Post post, User user);
        public bool CreateComment(Comment comment, User user);
        public bool UpdateComment(Comment comment, User user);
        public bool DeleteComment(Comment comment, User user);
        public bool VerifyPostUser(User user, Post post);
        public bool VerifyCommentUser(User user, Comment comment);
        public bool LogActivity(IPAddress ipAddress, string activity, User user);
        public List<ActivityLog> GetActivityLog(User user);
        public List<Post> SearchPost(string searchTerm);
    }
}