using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using forumx_server.Database;
using forumx_server.Email;
using forumx_server.Helper;
using forumx_server.Model;
using forumx_server.OauthVerifier;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace forumx_server.Auth
{
    public class LocalAuth : IAuthHandler
    {
        private readonly HashSet<string> _badUsernames;
        private readonly IConfiguration _configuration;
        private readonly IDatabase _database;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<LocalAuth> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IOauthProvider _oauthProvider;
        private readonly HashSet<string> _weakPasswords;

        public LocalAuth(IOauthProvider oauthProvider, IDatabase database, IEmailSender emailSender,
            IMemoryCache memoryCache, IConfiguration configuration, ILogger<LocalAuth> logger)
        {
            _oauthProvider = oauthProvider;
            _database = database;
            _emailSender = emailSender;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _logger = logger;
            _weakPasswords = new HashSet<string>(File.ReadAllLines(configuration["AuthWordList:BadPassword"]));
            _badUsernames = new HashSet<string>(File.ReadAllLines(configuration["AuthWordList:BadUsername"]));
        }

        public User VerifyOauthForRegister(string token)
        {
            var response = _oauthProvider.VerifyUserFromOauthToken(token, OauthActionEnum.Register);
            if (response == null)
            {
                _logger.LogInformation("Oauth handler rejected token.");
                return null;
            }

            if (!VerifyNonDuplicateEmail(response))
            {
                _logger.LogInformation($"Email {response.Email} already exists.");
                return null;
            }

            return response;
        }

        public string GenerateTotp(ref User user)
        {
            user.TotpToken = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(user.TotpToken);
            return OtpHelper.GenerateTotp(user.TotpToken);
        }


        public bool RegisterUser(ref User user)
        {
            if (user.TotpToken == null)
            {
                _logger.LogInformation("TOTP token has not been generated.");
                return false;
            }

            if (!VerifyPassword(user.Password))
            {
                _logger.LogInformation("Password verification failed.");
                return false;
            }

            if (!VerifyUsername(user.Username))
            {
                _logger.LogInformation("Username verification failed.");
                return false;
            }

            user.PasswordHash = Pbkdf2Password.PasswordToHash(user.Password);
            user.Uuid = SecureGuid.CreateSecureRfc4122Guid();
            _database.AddUser(user);
            _emailSender.SendRegisterEmail(user);
            return true;
        }

        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            if (password.Length > 64)
            {
                _logger.LogInformation("Password is too long.");
                return false;
            }

            if (password.Length < 8)
            {
                _logger.LogInformation("Password too short.");
                return false;
            }

            if (_weakPasswords.Contains(password))
            {
                _logger.LogInformation("Password in banned list.");
                return false;
            }

            return true;
        }

        public bool ChangePassword(User user, string newPassword)
        {
            if (!LoginUser(ref user))
            {
                _logger.LogInformation("User verification failed.");
                return false;
            }

            if (!VerifyPassword(newPassword))
            {
                _logger.LogInformation("Password verification failed.");
                return false;
            }

            user.PasswordHash = Pbkdf2Password.PasswordToHash(newPassword);
            _database.ChangePassword(user);
            _emailSender.SendChangePasswordEmail(user);
            return true;
        }

        public User UserFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var jwtUserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = new User
            {
                Uuid = new Guid(jwtUserId)
            };

            return user;
        }

        public List<ActivityLog> GetActivityLogs(User user)
        {
            return _database.GetActivityLog(user);
        }

        public bool ResetPassword(ref User user, string token)
        {
            var oauthUser = _oauthProvider.VerifyUserFromOauthToken(token, OauthActionEnum.Reset);
            if (oauthUser == null)
            {
                _logger.LogInformation("Oauth handler rejected token");
                return false;
            }

            var userNameUser = _database.GetUser(user.Username);
            if (userNameUser == null)
            {
                _logger.LogInformation("Username provided does not map to an account.");
                return false;
            }

            if (userNameUser.Email != oauthUser.Email)
            {
                _logger.LogInformation("Attempting to reset incorrect account.");
                return false;
            }

            if (!VerifyPassword(user.Password))
            {
                _logger.LogInformation("Password verification failed.");
                return false;
            }

            user.PasswordHash = Pbkdf2Password.PasswordToHash(user.Password);
            user.Uuid = userNameUser.Uuid;
            _database.ChangePassword(user);
            _emailSender.SendChangePasswordEmail(user);
            return true;
        }

        public bool VerifyUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            if (username.Length < 6)
            {
                _logger.LogInformation("Username is too short.");
                return false;
            }

            if (username.Length > 50)
            {
                _logger.LogInformation("Username is too long.");
                return false;
            }

            if (!username.All(char.IsLetterOrDigit))
            {
                _logger.LogInformation("Username contains special chars.");
                return false;
            }

            username = username.ToLower();

            if (_badUsernames.Contains(username))
            {
                _logger.LogInformation("Username in blocked list.");
                return false;
            }

            var user = _database.GetUser(username);
            if (user != null)
            {
                _logger.LogInformation("Username exists.");
                return false;
            }

            return true;
        }

        public bool LoginUser(ref User user)
        {
            User valUser;
            if (user.Username != null)
            {
                valUser = _database.GetUser(user.Username);
            }
            else if (user.Uuid != Guid.Empty)
            {
                valUser = _database.GetUser(user.Uuid);
            }
            else
            {
                _logger.LogInformation("Both username and uuid is null.");
                return false;
            }

            if (user.Password.Length > 64)
            {
                _logger.LogInformation("Password exceeds permitted length.");
                return false;
            }

            var randomBytes = new byte[48];
            RandomNumberGenerator.Fill(randomBytes);
            if (valUser == null)
            {
                _logger.LogInformation("Username does not exist.");
                Pbkdf2Password.CheckPasswordHash(randomBytes, "blahblahblah123");
                return false;
            }

            if (!Pbkdf2Password.CheckPasswordHash(valUser.PasswordHash, user.Password))
            {
                _logger.LogInformation("Password is incorrect.");
                return false;
            }

            user = valUser;
            user.Password = null;
            return true;
        }

        public TokenInfo VerifyOtp(User user, string otp)
        {
            if (OtpHelper.VerifyOtp(user.TotpToken, otp)) return GenerateToken(user);

            _logger.LogInformation("OTP failed verification.");
            return null;
        }

        public TokenInfo GenerateToken(User user)
        {
            if (!_memoryCache.TryGetValue(user.Uuid.ToString(), out Guid sessionGuid))
            {
                sessionGuid = SecureGuid.CreateSecureRfc4122Guid();
                var options = new MemoryCacheEntryOptions
                {
                    Priority = CacheItemPriority.NeverRemove
                };
                _memoryCache.Set(user.Uuid.ToString(), sessionGuid, options);
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Uuid.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,
                    SecureGuid.CreateSecureRfc4122Guid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, sessionGuid.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
                _configuration["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            var tokenInfo = new TokenInfo
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                Expiry = token.ValidTo
            };
            return tokenInfo;
        }

        public void TerminateSession(User user)
        {
            _memoryCache.Remove(user.Uuid.ToString());
        }

        public bool VerifyNonDuplicateEmail(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Email)) return false;

            return !_database.CheckEmailExists(user.Email);
        }
    }
}