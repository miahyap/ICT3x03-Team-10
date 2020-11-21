using System;
using System.Net;
using System.Security.Claims;
using forumx_server.Auth;
using forumx_server.Captcha;
using forumx_server.Helper;
using forumx_server.Logging;
using forumx_server.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace forumx_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IActivityLogger _activityLogger;
        private readonly IAuthHandler _authHandler;
        private readonly ICaptcha _captcha;
        private readonly ILogger<AccountController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IMemoryCache memoryCache, IAuthHandler authHandler,
            IWebHostEnvironment webHostEnvironment, IActivityLogger activityLogger, ILogger<AccountController> logger,
            ICaptcha captcha)
        {
            _memoryCache = memoryCache;
            _authHandler = authHandler;
            _webHostEnvironment = webHostEnvironment;
            _activityLogger = activityLogger;
            _logger = logger;
            _captcha = captcha;
        }

        // GET: api/<AccountController>
        [HttpPost("CheckPassword")]
        public IActionResult CheckPassword(TokenInput tokenInput)
        {
            if (Guid.TryParse(tokenInput.Token, out _) &&
                _memoryCache.TryGetValue("R1" + tokenInput.Token, out User user))
            {
                if (_memoryCache.TryGetValue("CHKPASS" + user.Email, out string _))
                {
                    return new ContentResult
                    {
                        StatusCode = (int?) HttpStatusCode.TooManyRequests
                    };
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
                _memoryCache.Set("CHKPASS" + user.Email, "null", cacheEntryOptions);

                return Ok(new {status = _authHandler.VerifyPassword(tokenInput.Data)});
            }

            _logger.LogInformation("Token is invalid.");
            return BadRequest();
        }

        [Authorize]
        [HttpPost("ChangePasswordCheck")]
        public IActionResult ChangePasswordCheck(User userInput)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);
            if (string.IsNullOrWhiteSpace(userInput.Password))
            {
                _logger.LogInformation("Password is null or empty");
                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);
            }

            return Ok(new {status = _authHandler.VerifyPassword(userInput.Password)});
        }

        [HttpPost("CheckUsername")]
        public IActionResult CheckUsername(TokenInput tokenInput)
        {
            if (string.IsNullOrWhiteSpace(tokenInput.Data) || string.IsNullOrWhiteSpace(tokenInput.Captcha) ||
                string.IsNullOrWhiteSpace(tokenInput.Token))
            {
                _logger.LogInformation("Token or data or captcha is empty or null.");
                return BadRequest();
            }

            if (!_captcha.VerifyCaptcha(tokenInput.Captcha, HttpContext.Connection.RemoteIpAddress,
                "checkUsername"))
            {
                _logger.LogInformation("Invalid Captcha.");
                return BadRequest();
            }

            if (Guid.TryParse(tokenInput.Token, out _) &&
                _memoryCache.TryGetValue("R1" + tokenInput.Token, out User user))
            {
                if (_memoryCache.TryGetValue("CHKUSRN" + user.Email, out string _))
                {
                    return new ContentResult
                    {
                        StatusCode = (int?) HttpStatusCode.TooManyRequests
                    };
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(5));
                _memoryCache.Set("CHKUSRN" + user.Email, "null", cacheEntryOptions);

                return Ok(new {status = _authHandler.VerifyUsername(tokenInput.Data.ToLower())});
            }

            _logger.LogInformation("Token is invalid.");
            return BadRequest();
        }

        [HttpPost("RegisterTest")]
        public IActionResult RegisterTest()
        {
            if (_webHostEnvironment.IsDevelopment())
            {
                var user = new User
                {
                    Uuid = SecureGuid.CreateSecureRfc4122Guid(),
                    Username = "TEST-TOKEN-USER-NOT-FOR-ACTUAL-USE"
                };

                var token = _authHandler.GenerateToken(user);
                return Ok(token);
            }

            return NotFound();
        }

        [HttpPost("GenerateOTP")]
        public IActionResult GenerateOtp(TokenInput input)
        {
            if (Guid.TryParse(input.Token, out _) &&
                _memoryCache.TryGetValue("R1" + input.Token, out User cachedUser))
            {
                var qrCode = _authHandler.GenerateTotp(ref cachedUser);
                var response = new MessageResponse
                {
                    Message = qrCode,
                    Status = "Ok"
                };

                return Ok(response);
            }

            _logger.LogInformation("Token is invalid.");
            return BadRequest();
        }


        [HttpPost("RegisterAccount")]
        public IActionResult RegisterAccount(RegisterUserInput inputUser)
        {
            if (string.IsNullOrWhiteSpace(inputUser.Username) || string.IsNullOrWhiteSpace(inputUser.Password) ||
                string.IsNullOrWhiteSpace(inputUser.Captcha))
            {
                _logger.LogInformation("Username, password or captcha is empty.");
                return BadRequest();
            }

            if (inputUser.Username.Length > 50)
            {
                _logger.LogInformation("Username exceeds permitted length.");
                return BadRequest();
            }

            if (!_captcha.VerifyCaptcha(inputUser.Captcha, HttpContext.Connection.RemoteIpAddress, "register"))
            {
                _logger.LogInformation("Captcha failed verification");
                return BadRequest();
            }

            if (Guid.TryParse(inputUser.Token, out _) &&
                _memoryCache.TryGetValue("R1" + inputUser.Token, out User cachedUser))
            {
                _memoryCache.Remove("R1" + inputUser.Token);
                cachedUser.Password = inputUser.Password;
                cachedUser.Username = inputUser.Username.ToLower();
                if (_authHandler.RegisterUser(ref cachedUser))
                {
                    _activityLogger.LogRegister(Request.HttpContext.Connection.RemoteIpAddress, cachedUser);
                    return Ok();
                }

                _logger.LogInformation("Auth handler rejected account.");
                return BadRequest();
            }

            _logger.LogInformation("Token is invalid.");
            return BadRequest();
        }

        [HttpPost("VerifyIdentity")]
        public IActionResult VerifyAzureAdCode(TokenInput tokenInput)
        {
            if (string.IsNullOrWhiteSpace(tokenInput.Token))
            {
                _logger.LogInformation("Token is empty.");
                return BadRequest();
            }

            var user = _authHandler.VerifyOauthForRegister(tokenInput.Token);
            if (user == null)
            {
                _logger.LogInformation("Auth handler rejected oauth token.");
                return BadRequest();
            }

            if (_memoryCache.TryGetValue("R1" + user.Email, out string stringUuid))
                _memoryCache.Remove("R1" + stringUuid);

            var registerGuid = SecureGuid.CreateSecureRfc4122Guid();
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            _memoryCache.Set("R1" + registerGuid, user, cacheEntryOptions);
            _memoryCache.Set("R1" + user.Email, registerGuid.ToString(), cacheEntryOptions);

            var response = new OauthSuccessResponse
            {
                Name = user.Name,
                Email = user.Email,
                RegisterCode = registerGuid.ToString()
            };

            return Ok(response);
        }

        [HttpPost("ResetPassword")]
        public IActionResult ResetPasswordOauth(ResetPasswordInput resetPasswordInput)
        {
            if (!_captcha.VerifyCaptcha(resetPasswordInput.Captcha, HttpContext.Connection.RemoteIpAddress,
                "resetpassword"))
            {
                _logger.LogInformation("Invalid Captcha.");
                return BadRequest();
            }

            var user = new User
            {
                Username = resetPasswordInput.Username,
                Password = resetPasswordInput.Password
            };

            if (_authHandler.ResetPassword(ref user, resetPasswordInput.Token))
            {
                _activityLogger.LogResetPassword(Request.HttpContext.Connection.RemoteIpAddress, user);
                return Ok();
            }

            _logger.LogInformation("Auth handler rejected account.");
            return BadRequest();
        }


        [HttpPost("Login")]
        public IActionResult Login(LoginInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
            {
                _logger.LogInformation("Username or password is null or empty");
                return Unauthorized();
            }

            if (input.Username.Length > 50)
            {
                _logger.LogInformation("Username exceeds permitted length.");
                return Unauthorized();
            }

            if (input.Password.Length > 64)
            {
                _logger.LogInformation("Password exceeds permitted length.");
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(input.Captcha))
            {
                _logger.LogInformation("Captcha is null or empty");
                return Unauthorized();
            }

            if (!_captcha.VerifyCaptcha(input.Captcha, Request.HttpContext.Connection.RemoteIpAddress, "login"))
            {
                _logger.LogInformation("Captcha rejected.");
                return Unauthorized();
            }

            var user = new User
            {
                Username = input.Username,
                Password = input.Password
            };


            if (_authHandler.LoginUser(ref user))
            {
                var registerGuid = SecureGuid.CreateSecureRfc4122Guid();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                _memoryCache.Set("L1" + registerGuid, user, cacheEntryOptions);

                var response = new MessageResponse
                {
                    Message = registerGuid.ToString(),
                    Status = "Ok"
                };
                return Ok(response);
            }

            _logger.LogInformation("Auth handler rejected login.");
            return Unauthorized();
        }

        [HttpPost("VerifyOTP")]
        public IActionResult VerifyOtp(TokenInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Token) || string.IsNullOrWhiteSpace(input.Data))
            {
                _logger.LogInformation("Token or OTP is null or empty");
                return BadRequest();
            }


            if (Guid.TryParse(input.Token, out _) && _memoryCache.TryGetValue("L1" + input.Token, out User cachedUser))
            {
                var token = _authHandler.VerifyOtp(cachedUser, input.Data);
                if (token != null)
                {
                    _activityLogger.LogLogin(Request.HttpContext.Connection.RemoteIpAddress, cachedUser);
                    return Ok(token);
                }

                _logger.LogInformation("Auth handler rejected OTP.");
                return BadRequest();
            }

            _logger.LogInformation("Token is invalid.");
            return BadRequest();
        }

        [Authorize]
        [HttpPost("TerminateSession")]
        public IActionResult TerminateSession()
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);
            _authHandler.TerminateSession(user);
            _activityLogger.LogTerminateSession(Request.HttpContext.Connection.RemoteIpAddress, user);
            return Ok();
        }

        [Authorize]
        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            var tokenExpiry = User.FindFirstValue("exp");
            var currentEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (currentEpoch - long.Parse(tokenExpiry) < 900)
            {
                var user = new User
                {
                    Uuid = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    Username = User.FindFirstValue(ClaimTypes.Name)
                };

                var token = _authHandler.GenerateToken(user);
                return Ok(token);
            }

            _logger.LogInformation("Token is not expiring soon.");
            return Unauthorized();
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(ChangePassInput changePassInput)
        {
            var user = _authHandler.UserFromClaimsPrincipal(User);

            if (string.IsNullOrWhiteSpace(changePassInput.OldPassword) ||
                string.IsNullOrWhiteSpace(changePassInput.NewPassword))
            {
                _logger.LogInformation("Old or new Password is null or empty.");

                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection.RemoteIpAddress.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            if (changePassInput.OldPassword.Length > 64 || changePassInput.NewPassword.Length > 64)
            {
                _logger.LogInformation("Old or new password length exceeds permitted length.");

                _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                       $", IP: {HttpContext?.Connection.RemoteIpAddress.ToString() ?? "Unknown IP"}");
                _authHandler.TerminateSession(user);

                return BadRequest();
            }

            user.Password = changePassInput.OldPassword;

            if (_authHandler.ChangePassword(user, changePassInput.NewPassword))
            {
                _activityLogger.LogChangePassword(Request.HttpContext.Connection.RemoteIpAddress, user);
                return Ok();
            }

            _logger.LogInformation("Auth handler rejected password change.");
            _logger.LogInformation($"Terminating session. User: {user.Uuid}" +
                                   $", IP: {HttpContext?.Connection.RemoteIpAddress.ToString() ?? "Unknown IP"}");
            _authHandler.TerminateSession(user);
            return BadRequest();
        }

        [Authorize]
        [HttpGet("ActivityLogs")]
        public IActionResult GetActivityLogs()
        {
            return Ok(_authHandler.GetActivityLogs(_authHandler.UserFromClaimsPrincipal(User)));
        }

        [Authorize]
        [HttpGet("CredsTest")]
        public IActionResult TestCreds()
        {
            return Ok(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}