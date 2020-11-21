using System;
using forumx_server.Auth;
using forumx_server.Database;
using forumx_server.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace forumx_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IAuthHandler _authHandler;
        private readonly IDatabase _database;
        private readonly ILogger<TopicController> _logger;

        public TopicController(IDatabase database, ILogger<TopicController> logger, IAuthHandler authHandler)
        {
            _database = database;
            _logger = logger;
            _authHandler = authHandler;
        }

        // GET: api/<Topic>
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_database.GetTopics(null));
        }

        // GET api/<Topic>/5
        [Authorize]
        [HttpGet("{uuid}")]
        public IActionResult Get(string uuid)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);
            if (!SecureGuid.VerifyGuid(uuid, out _))
            {
                _logger.LogInformation("Invalid Topic UUID");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection.RemoteIpAddress.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            var topicInfo = _database.GetTopics(uuid);
            if (topicInfo.Count != 1)
            {
                _logger.LogInformation("Topic UUID does nto exist");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection.RemoteIpAddress.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
            }

            var posts = _database.GetPostByTopic(new Guid(uuid));
            topicInfo[0].Posts = posts;
            return Ok(topicInfo[0]);
        }
    }
}