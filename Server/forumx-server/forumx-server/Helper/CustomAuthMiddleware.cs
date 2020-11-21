using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace forumx_server.Helper
{
    public class CustomAuthMiddleWare
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly RequestDelegate _next;

        public CustomAuthMiddleWare(RequestDelegate next, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _next = next;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachAccountToContext(context, token);

            await _next(context);
        }

        private void AttachAccountToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Tokens:key"]);
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _configuration["Tokens:Issuer"],
                    ValidAudience = _configuration["Tokens:Issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;
                var userUuid = jwtToken.Claims.First(x => x.Type == "sub").Value;

                if (_memoryCache.TryGetValue(userUuid, out Guid sessionGuid))
                {
                    var tokenSid = new Guid(jwtToken.Claims.First(x => x.Type == "sid").Value);
                    if (sessionGuid == tokenSid) context.User = claimsPrincipal;
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}