using System.Security.Claims;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace forumx_server.Helper
{
    public class CustomClientRateLimitMiddleware : ClientRateLimitMiddleware
    {
        private readonly IRateLimitConfiguration _config;
        private readonly ILogger<CustomClientRateLimitMiddleware> _logger;

        public CustomClientRateLimitMiddleware(RequestDelegate next,
            IOptions<ClientRateLimitOptions> options,
            IRateLimitCounterStore counterStore,
            IClientPolicyStore policyStore,
            IRateLimitConfiguration config,
            ILogger<CustomClientRateLimitMiddleware> logger) : base(next, options, counterStore, policyStore, config,
            logger)
        {
            _config = config;
            _logger = logger;
        }

        public override ClientRequestIdentity ResolveIdentity(HttpContext httpContext)
        {
            var user = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var identity = base.ResolveIdentity(httpContext);
            identity.ClientId = user;
            return identity;
        }
    }
}