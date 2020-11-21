using System.Collections.Generic;
using System.Security.Claims;
using forumx_server.Model;

namespace forumx_server.Auth
{
    public interface IAuthHandler
    {
        public User VerifyOauthForRegister(string token);
        public string GenerateTotp(ref User user);
        public bool RegisterUser(ref User user);
        public bool LoginUser(ref User user);
        public bool ResetPassword(ref User user, string token);
        public bool VerifyUsername(string username);
        public TokenInfo VerifyOtp(User user, string otp);
        public TokenInfo GenerateToken(User user);
        public void TerminateSession(User user);
        bool ChangePassword(User user, string newPassword);
        public User UserFromClaimsPrincipal(ClaimsPrincipal claimsPrincipal);
        public bool VerifyPassword(string password);
        public List<ActivityLog> GetActivityLogs(User user);
    }
}