using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using forumx_server.Helper;
using forumx_server.Model;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace forumx_server.Database
{
    public class MariaDatabase : IDatabase
    {
        private readonly MySqlConnectionStringBuilder _connectionStringBuilder;

        public MariaDatabase(IConfiguration configuration)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = configuration["MariaDB:Server"],
                Database = configuration["MariaDB:Database"],
                UserID = configuration["MariaDB:UserID"],
                Password = configuration["MariaDB:Password"]
            };

            _connectionStringBuilder = builder;
        }

        public void AddUser(User user)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "addUser";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@in_uuid", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            command.Parameters.AddWithValue("@in_username", user.Username);
            command.Parameters.Add("@in_passHash", MySqlDbType.Binary, 48).Value = user.PasswordHash;
            command.Parameters.Add("@in_totp", MySqlDbType.Binary, 32).Value = user.TotpToken;
            command.Parameters.AddWithValue("@in_email", user.Email);
            command.Parameters.AddWithValue("@in_actName", user.Name);
            command.ExecuteNonQuery();
        }

        public User GetUser(string username)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "getUserByLogin";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@in_username", username);
            var dataReader = command.ExecuteReader();
            User result = null;
            if (dataReader.HasRows)
            {
                dataReader.Read();
                result = new User
                {
                    ActivatedUser = true,
                    Email = dataReader["email"].ToString(),
                    PasswordHash = dataReader["passHash"] as byte[],
                    Name = dataReader["name"].ToString(),
                    TotpToken = dataReader["totp"] as byte[],
                    Username = dataReader["login"].ToString(),
                    Uuid = new Guid(dataReader["uuid"] as byte[] ?? throw new InvalidOperationException())
                };
            }

            return result;
        }

        public User GetUser(Guid userGuid)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "getUserByGuid";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@in_userGuid", MySqlDbType.Binary, 16).Value = userGuid.ToByteArray();
            var dataReader = command.ExecuteReader();
            User result = null;
            if (dataReader.HasRows)
            {
                dataReader.Read();
                result = new User
                {
                    ActivatedUser = true,
                    Email = dataReader["email"].ToString(),
                    PasswordHash = dataReader["passHash"] as byte[],
                    Name = dataReader["name"].ToString(),
                    TotpToken = dataReader["totp"] as byte[],
                    Username = dataReader["login"].ToString(),
                    Uuid = new Guid(dataReader["uuid"] as byte[] ?? throw new InvalidOperationException())
                };
            }

            return result;
        }

        public bool CheckEmailExists(string email)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "checkEmailExists";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@inEmail", email);
            var dataReader = command.ExecuteReader();
            return dataReader.HasRows;
        }

        public List<Topic> GetTopics(string topicUuid)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "getTopics";
            command.CommandType = CommandType.StoredProcedure;
            if (topicUuid == null)
            {
                command.Parameters.Add("@topicUUID", MySqlDbType.Binary, 16).Value = null;
            }
            else
            {
                var topicGuid = new Guid(topicUuid);
                command.Parameters.Add("@topicUUID", MySqlDbType.Binary, 16).Value = topicGuid.ToByteArray();
            }

            var dataReader = command.ExecuteReader();
            var result = new List<Topic>();

            while (dataReader.Read())
            {
                var rowTopic = new Topic
                {
                    Name = dataReader["name"].ToString(),
                    Uuid = new Guid(dataReader["uuid"] as byte[] ?? throw new InvalidOperationException()).ToString()
                };

                result.Add(rowTopic);
            }

            return result;
        }

        public List<Post> GetPostByTopic(Guid topicUuid)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "getPostsByTopic";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@topicUUID", MySqlDbType.Binary, 16).Value = topicUuid.ToByteArray();

            var dataReader = command.ExecuteReader();
            var result = new List<Post>();

            while (dataReader.Read())
            {
                var rowPost = new Post
                {
                    Content = dataReader["content"].ToString(),
                    PostedTime = (DateTime) dataReader["timePosted"],
                    User = dataReader["login"].ToString(),
                    Title = dataReader["title"].ToString(),
                    Edited = (bool) dataReader["edited"],
                    Uuid = new Guid(dataReader["uuid"] as byte[] ?? throw new InvalidOperationException()).ToString()
                };

                result.Add(rowPost);
            }

            return result;
        }

        public List<Comment> GetCommentsByPost(Guid postUuid)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "getCommentsByPost";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@postUUID", MySqlDbType.Binary, 16).Value = postUuid.ToByteArray();

            var dataReader = command.ExecuteReader();
            var result = new List<Comment>();

            while (dataReader.Read())
            {
                var rowComment = new Comment
                {
                    Content = dataReader["content"].ToString(),
                    User = dataReader["login"].ToString(),
                    Edited = (bool) dataReader["edited"],
                    Uuid = new Guid(dataReader["uuid"] as byte[] ?? throw new InvalidOperationException()).ToString(),
                    PostedTime = (DateTime) dataReader["timePosted"]
                };

                result.Add(rowComment);
            }

            return result;
        }

        public bool UpdatePost(Post post, User user)
        {
            if (!SecureGuid.VerifyGuid(post.Uuid, out var postGuid)) return false;

            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "editPost";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@postUUID", MySqlDbType.Binary, 16).Value = postGuid.ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            command.Parameters.Add("@updatedContent", MySqlDbType.Text).Value = post.Content;
            var response = command.ExecuteNonQuery();

            return response == 2;
        }

        public bool DeletePost(Post post, User user)
        {
            if (!SecureGuid.VerifyGuid(post.Uuid, out var postGuid)) return false;


            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "deletePost";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@postUUID", MySqlDbType.Binary, 16).Value = postGuid.ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            var result = command.ExecuteNonQuery();

            return result == 2;
        }

        public bool CreatePost(Post post, User user)
        {
            if (!SecureGuid.VerifyGuid(post.Topic, out var topicGuid)) return false;

            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();
            command.CommandText = "newPost";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@postUUID", MySqlDbType.Binary, 16).Value =
                SecureGuid.CreateSecureRfc4122Guid().ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            command.Parameters.Add("@topicUUID", MySqlDbType.Binary, 16).Value = topicGuid.ToByteArray();
            command.Parameters.AddWithValue("@postTitle", post.Title);
            command.Parameters.AddWithValue("@postContent", post.Content);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public bool CreateComment(Comment comment, User user)
        {
            if (!SecureGuid.VerifyGuid(comment.Post, out var postGuid)) return false;

            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();
            command.CommandText = "newComment";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@postUUID", MySqlDbType.Binary, 16).Value = postGuid.ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            command.Parameters.Add("@commentUUID", MySqlDbType.Binary, 16).Value =
                SecureGuid.CreateSecureRfc4122Guid().ToByteArray();
            command.Parameters.AddWithValue("@commentContent", comment.Content);
            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }

        public bool UpdateComment(Comment comment, User user)
        {
            if (!SecureGuid.VerifyGuid(comment.Uuid, out var commentGuid)) return false;

            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "editComment";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@commentUUID", MySqlDbType.Binary, 16).Value = commentGuid.ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            command.Parameters.Add("@updatedContent", MySqlDbType.Text).Value = comment.Content;
            var response = command.ExecuteNonQuery();

            return response == 2;
        }

        public bool DeleteComment(Comment comment, User user)
        {
            if (!SecureGuid.VerifyGuid(comment.Uuid, out var commentGuid)) return false;

            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "deleteComment";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@commentUUID", MySqlDbType.Binary, 16).Value = commentGuid.ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            var result = command.ExecuteNonQuery();

            return result == 2;
        }

        public bool VerifyPostUser(User user, Post post)
        {
            if (!SecureGuid.VerifyGuid(post.Uuid, out var postGuid)) return false;

            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "verifyPostUser";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@postUUID", MySqlDbType.Binary, 16).Value = postGuid.ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            var result = command.ExecuteReader();

            return result.HasRows;
        }

        public bool VerifyCommentUser(User user, Comment comment)
        {
            if (!SecureGuid.VerifyGuid(comment.Uuid, out var commentGuid)) return false;

            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "verifyCommentUser";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@commentUUID", MySqlDbType.Binary, 16).Value = commentGuid.ToByteArray();
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            var result = command.ExecuteReader();

            return result.HasRows;
        }

        public bool LogActivity(IPAddress ipAddress, string activity, User user)
        {
            var ipString = ipAddress == null ? "Invalid IP" : ipAddress.ToString();
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "newActivity";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            command.Parameters.Add("@inSource", MySqlDbType.VarChar, 50).Value = ipString;
            command.Parameters.Add("@inActivity", MySqlDbType.Text).Value = activity;
            _ = command.ExecuteNonQuery();

            return true;
        }

        public List<ActivityLog> GetActivityLog(User user)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "getActivityLogs";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();

            var dataReader = command.ExecuteReader();
            var result = new List<ActivityLog>();

            while (dataReader.Read())
            {
                var log = new ActivityLog
                {
                    Source = dataReader["source"].ToString(),
                    Time = (DateTime) dataReader["time"],
                    Activity = dataReader["activity"].ToString()
                };
                result.Add(log);
            }

            return result;
        }

        public Post GetPostInfo(Guid postGuid)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "getPostInfo";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@postUUID", MySqlDbType.Binary, 16).Value = postGuid.ToByteArray();

            var dataReader = command.ExecuteReader();
            Post result = null;

            if (dataReader.HasRows)
            {
                dataReader.Read();
                result = new Post
                {
                    Title = dataReader["title"].ToString(),
                    Content = dataReader["content"].ToString(),
                    PostedTime = (DateTime) dataReader["timePosted"],
                    Edited = (bool) dataReader["edited"],
                    Topic = dataReader["topic"].ToString(),
                    User = dataReader["user"].ToString()
                };
            }

            return result;
        }

        public void ChangePassword(User user)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "changePassword";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@userUUID", MySqlDbType.Binary, 16).Value = user.Uuid.ToByteArray();
            command.Parameters.Add("@newPassHash", MySqlDbType.Binary, 48).Value = user.PasswordHash;
            command.ExecuteNonQuery();
        }

        public List<Post> SearchPost(string searchTerm)
        {
            using var conn = new MySqlConnection(_connectionStringBuilder.ConnectionString);
            conn.Open();
            using var command = conn.CreateCommand();

            command.CommandText = "searchPost";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@searchTerm", searchTerm);

            var dataReader = command.ExecuteReader();
            var result = new List<Post>();

            while (dataReader.Read())
            {
                var post = new Post
                {
                    Uuid = new Guid(dataReader["uuid"] as byte[] ?? throw new InvalidOperationException()).ToString(),
                    Title = dataReader["title"].ToString(),
                    Content = dataReader["content"].ToString(),
                    PostedTime = (DateTime) dataReader["timePosted"],
                    Edited = (bool) dataReader["edited"],
                    Topic = dataReader["topic"].ToString(),
                    User = dataReader["user"].ToString()
                };

                result.Add(post);
            }

            return result;
        }
    }
}