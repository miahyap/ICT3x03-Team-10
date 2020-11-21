using System.Collections.Generic;
using System.Net.Http;
using forumx_server.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace forumx_server.OauthVerifier
{
    public class MicrosoftOauthProvider : IOauthProvider
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly ILogger<MicrosoftOauthProvider> _logger;

        public MicrosoftOauthProvider(IConfiguration configuration, IHttpClientFactory clientFactory,
            ILogger<MicrosoftOauthProvider> logger)
        {
            _clientId = configuration["AzureAD:client_id"];
            _clientSecret = configuration["AzureAD:client_secret"];
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public User VerifyUserFromOauthToken(string token, OauthActionEnum actionEnum)
        {
            string redirectUri;
            if (actionEnum == OauthActionEnum.Register)
            {
                redirectUri = "https://forumx.sitict.net/register";
            }
            else if (actionEnum == OauthActionEnum.Reset)
            {
                redirectUri = "https://forumx.sitict.net/resetpassword";
            }
            else
            {
                _logger.LogInformation("Oauth verification failed. Unsupported action.");
                return null;
            }

            var authRequest = new HttpRequestMessage(HttpMethod.Post,
                "https://login.microsoftonline.com/common/oauth2/v2.0/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", _clientId),
                    new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/User.ReadBasic.All"),
                    new KeyValuePair<string, string>("code", token),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_secret", _clientSecret)
                })
            };

            var authClient = _clientFactory.CreateClient();
            var authResponse = authClient.SendAsync(authRequest).Result;
            if (authResponse.IsSuccessStatusCode)
            {
                var oauthResponse = authResponse.Content.ReadAsStringAsync().Result;
                var oauthJson = JToken.Parse(oauthResponse);
                var accessToken = oauthJson["access_token"];
                var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");
                userInfoRequest.Headers.Add("Authorization", $"Bearer {accessToken}");
                var userInfoClient = _clientFactory.CreateClient();
                var userInfoResponse = userInfoClient.SendAsync(userInfoRequest).Result;

                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    var responseMessage = userInfoResponse.Content.ReadAsStreamAsync().Result;
                    _logger.LogInformation($"Oauth verification failed while retrieving user info. {responseMessage}");
                    return null;
                }

                var userInfo = userInfoResponse.Content.ReadAsStringAsync().Result;
                var userInfoJson = JToken.Parse(userInfo);

                if (userInfoJson["mail"] == null ||
                    !userInfoJson["mail"].ToString().EndsWith("singaporetech.edu.sg"))
                {
                    _logger.LogInformation($"Oauth verification failed. Non-SIT email used. {userInfoJson["mail"]}");
                    return null;
                }

                if (userInfoJson["jobTitle"] == null ||
                    !userInfoJson["jobTitle"].ToString().ToLower().EndsWith("student"))

                {
                    _logger.LogInformation(
                        $"Oauth verification failed. Non-SIT student email used. {userInfoJson["mail"]}");
                    return null;
                }

                var response = new User
                {
                    Email = userInfoJson["mail"].ToString(),
                    Name = userInfoJson["displayName"].ToString(),
                    ActivatedUser = false
                };

                return response;
            }

            _logger.LogInformation(
                $"Oauth verification failed. Token failed to validate. {authResponse.Content.ReadAsStringAsync().Result}");
            return null;
        }
    }
}