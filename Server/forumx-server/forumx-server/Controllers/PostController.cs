using forumx_server.Auth;
using forumx_server.Captcha;
using forumx_server.Database;
using forumx_server.Helper;
using forumx_server.Logging;
using forumx_server.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace forumx_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IActivityLogger _activityLogger;
        private readonly IAuthHandler _authHandler;
        private readonly ICaptcha _captcha;
        private readonly IDatabase _database;
        private readonly ILogger<PostController> _logger;

        public PostController(IDatabase database, IAuthHandler authHandler, IActivityLogger activityLogger,
            ILogger<PostController> logger, ICaptcha captcha)
        {
            _database = database;
            _authHandler = authHandler;
            _activityLogger = activityLogger;
            _logger = logger;
            _captcha = captcha;
        }


        // GET api/<Post>/5
        [Authorize]
        [HttpGet("{uuid}")]
        public IActionResult Get(string uuid)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);
            if (!SecureGuid.VerifyGuid(uuid, out var postGuid))
            {
                _logger.LogInformation("Post UUID is invalid.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }


            var postInfo = _database.GetPostInfo(postGuid);
            if (postInfo == null)
            {
                _logger.LogInformation("Post does not exist.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection.RemoteIpAddress.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            var comments = _database.GetCommentsByPost(postGuid);
            postInfo.Comments = comments;

            return Ok(postInfo);
        }


        // PUT api/<Post>/5
        [Authorize]
        [HttpPut]
        public IActionResult UpdatePost(Post post)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);

            if (string.IsNullOrWhiteSpace(post.Content) || string.IsNullOrWhiteSpace(post.Uuid))
            {
                _logger.LogInformation("Post content or uuid is null or empty.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
                return BadRequest();
            }

            if (!SecureGuid.VerifyGuid(post.Uuid, out _))
            {
                _logger.LogInformation("Post UUID is invalid.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
                return BadRequest();
            }


            if (!_database.VerifyPostUser(user, post))
            {
                _logger.LogInformation("Requester is not post creator.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            if (_database.UpdatePost(post, user))
            {
                _activityLogger.LogEditPost(Request.HttpContext.Connection.RemoteIpAddress, user, post);
                return Ok();
            }

            _logger.LogInformation("DB failed to edit post.");
            _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                   $", IP: {HttpContext?.Connection.RemoteIpAddress.ToString() ?? "Unknown IP"}");
            _authHandler.TerminateSession(user);

            return BadRequest();
        }

        [Authorize]
        [HttpPost("NewPost")]
        public IActionResult NewPost(Post post)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);

            if (string.IsNullOrWhiteSpace(post.Topic) || string.IsNullOrWhiteSpace(post.Content) ||
                string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Captcha))
            {
                _logger.LogInformation("Topic, Content, Title or Captcha is null or empty.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            if (!_captcha.VerifyCaptcha(post.Captcha, HttpContext.Connection.RemoteIpAddress, "newPost"))
            {
                _logger.LogInformation("Captcha failed verification.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            if (post.Content.Length > 512 || post.Title.Length > 50)
            {
                _logger.LogInformation("Content or Title exceeds max permissible length.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }


            if (_database.CreatePost(post, user))
            {
                _activityLogger.LogNewPost(Request.HttpContext.Connection.RemoteIpAddress, user, post);
                return Ok();
            }

            _logger.LogInformation("DB failed to create post.");
            _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                   $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
            _authHandler.TerminateSession(user);

            return BadRequest();
        }

        // DELETE api/<Post>/5
        [Authorize]
        [HttpDelete("{postUUID}")]
        public IActionResult Delete(string postUuid)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);

            if (!SecureGuid.VerifyGuid(postUuid, out _))
            {
                _logger.LogInformation("Post UUID is invalid.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }


            var post = new Post
            {
                Uuid = postUuid
            };

            if (_database.DeletePost(post, user))
            {
                _activityLogger.LogDeletePost(Request.HttpContext.Connection.RemoteIpAddress, user, post);
                return Ok();
            }

            _logger.LogInformation("DB failed to delete post.");
            _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                   $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
            _authHandler.TerminateSession(user);

            return BadRequest();
        }
    }
}