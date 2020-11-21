using forumx_server.Auth;
using forumx_server.Captcha;
using forumx_server.Database;
using forumx_server.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace forumx_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IAuthHandler _authHandler;
        private readonly ICaptcha _captcha;
        private readonly IDatabase _database;
        private readonly ILogger<SearchController> _logger;

        public SearchController(IDatabase database, ILogger<SearchController> logger, IAuthHandler authHandler,
            ICaptcha captcha)
        {
            _database = database;
            _logger = logger;
            _authHandler = authHandler;
            _captcha = captcha;
        }

        // GET api/<SearchController>/5
        [Authorize]
        [HttpGet("{search}")]
        public IActionResult Get(string search)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);

            if (Request.Headers.TryGetValue("Captcha", out var captchaValue))
            {
                if (!_captcha.VerifyCaptcha(captchaValue, HttpContext.Connection.RemoteIpAddress, "search"))
                {
                    _logger.LogInformation("Captcha verification failed.");
                    _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                           $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                    _authHandler.TerminateSession(user);
                    return BadRequest();
                }
            }
            else
            {
                _logger.LogInformation("Captcha header not provided.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(search) || search.Length < 5)
            {
                _logger.LogInformation("Search input is empty or is less than 5 char.");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
                return BadRequest();
            }

            return Ok(_database.SearchPost(search));
        }
    }
}