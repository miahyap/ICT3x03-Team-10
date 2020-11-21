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
    public class CommentController : ControllerBase
    {
        private readonly IActivityLogger _activityLogger;
        private readonly IAuthHandler _authHandler;
        private readonly ICaptcha _captcha;
        private readonly IDatabase _database;
        private readonly ILogger<CommentController> _logger;

        public CommentController(IDatabase database, IAuthHandler authHandler, IActivityLogger activityLogger,
            ILogger<CommentController> logger, ICaptcha captcha)
        {
            _database = database;
            _authHandler = authHandler;
            _activityLogger = activityLogger;
            _logger = logger;
            _captcha = captcha;
        }

        // POST api/<CommentController>
        [Authorize]
        [HttpPost("NewComment")]
        public IActionResult NewComment([FromBody] Comment comment)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);

            if (string.IsNullOrWhiteSpace(comment.Content) || string.IsNullOrWhiteSpace(comment.Post) ||
                string.IsNullOrWhiteSpace(comment.Captcha))
            {
                _logger.LogInformation("Comment content, post or captcha is missing.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            if (!_captcha.VerifyCaptcha(comment.Captcha, HttpContext.Connection.RemoteIpAddress, "newComment"))
            {
                _logger.LogInformation("Captcha failed verification.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }


            if (comment.Content.Length > 128)
            {
                _logger.LogInformation("Comment content length exceeds the permitted limit.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            if (!SecureGuid.VerifyGuid(comment.Post, out _))
            {
                _logger.LogInformation("Post UUID is invalid.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }


            if (_database.CreateComment(comment, user))
            {
                _activityLogger.LogNewComment(Request.HttpContext.Connection.RemoteIpAddress, user, comment);
                return Ok();
            }

            _logger.LogInformation("Database failed to create new comment.");
            _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                   $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
            _authHandler.TerminateSession(user);

            return BadRequest();
        }

        // PUT api/<CommentController>/5
        [Authorize]
        [HttpPut]
        public IActionResult Put(Comment comment)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);
            if (string.IsNullOrWhiteSpace(comment.Uuid) || string.IsNullOrWhiteSpace(comment.Content))
            {
                _logger.LogInformation("Comment uuid or content is empty.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
                return BadRequest();
            }

            if (!SecureGuid.VerifyGuid(comment.Uuid, out _))
            {
                _logger.LogInformation("Comment UUID is invalid.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            if (!_database.VerifyCommentUser(user, comment))
            {
                _logger.LogInformation("Requester is not comment creator.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
                return BadRequest();
            }

            if (_database.UpdateComment(comment, user))
            {
                _activityLogger.LogEditComment(Request.HttpContext.Connection.RemoteIpAddress, user, comment);
                return Ok();
            }

            _logger.LogInformation("Database failed to update comment.");
            _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                   $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
            _authHandler.TerminateSession(user);
            return BadRequest();
        }

        // DELETE api/<CommentController>/5
        [Authorize]
        [HttpDelete("{commentUUID}")]
        public IActionResult Delete(string commentUuid)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);

            if (!SecureGuid.VerifyGuid(commentUuid, out _))
            {
                _logger.LogInformation("Comment UUID is invalid.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            var comment = new Comment
            {
                Uuid = commentUuid
            };
            if (_database.DeleteComment(comment, user))
            {
                _activityLogger.LogDeleteComment(Request.HttpContext.Connection.RemoteIpAddress, user, comment);
                return Ok();
            }

            _logger.LogInformation("Database failed to delete comment.");
            _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                   $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
            _authHandler.TerminateSession(user);
            return BadRequest();
        }
    }
}