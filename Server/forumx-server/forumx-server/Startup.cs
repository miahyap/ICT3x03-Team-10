using System.Net;
using AspNetCoreRateLimit;
using forumx_server.Auth;
using forumx_server.Captcha;
using forumx_server.Database;
using forumx_server.Email;
using forumx_server.Helper;
using forumx_server.Logging;
using forumx_server.OauthVerifier;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace forumx_server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddMemoryCache();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                options.KnownProxies.Add(IPAddress.Parse("::ffff:172.17.0.1"));
            });


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>();

            services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            //services.Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"));

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddSingleton<IOauthProvider, MicrosoftOauthProvider>();
            services.Configure<MicrosoftOauthProvider>(Configuration);

            services.AddSingleton<IDatabase, MariaDatabase>();
            services.Configure<MariaDatabase>(Configuration);

            services.AddSingleton<IEmailSender, SendGridEmailSender>();
            services.Configure<SendGridEmailSender>(Configuration);

            services.AddSingleton<IAuthHandler, LocalAuth>();
            services.Configure<LocalAuth>(Configuration);

            services.AddSingleton<IActivityLogger, LocalActivityLog>();
            services.Configure<LocalActivityLog>(Configuration);

            services.AddSingleton<ICaptcha, ReCaptcha>();
            services.Configure<ReCaptcha>(Configuration);

            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });

                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<CustomAuthMiddleWare>();

            app.UseMiddleware<CustomClientRateLimitMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}