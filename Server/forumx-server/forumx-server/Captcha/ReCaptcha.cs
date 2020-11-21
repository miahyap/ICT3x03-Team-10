using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace forumx_server.Captcha
{
    public class ReCaptcha : ICaptcha
    {
        private readonly string _captchaSecret;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ReCaptcha> _logger;


        public ReCaptcha(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<ReCaptcha> logger)
        {
            _captchaSecret = configuration["reCaptcha:Secret"];
            _clientFactory = httpClientFactory;
            _logger = logger;
        }

        public bool VerifyCaptcha(string captchaResponse, IPAddress ipAddress, string action)
        {
            var verifyRequest = new HttpRequestMessage(HttpMethod.Post,
                "https://www.google.com/recaptcha/api/siteverify")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", _captchaSecret),
                    new KeyValuePair<string, string>("response", captchaResponse),
                    new KeyValuePair<string, string>("remoteip", ipAddress.ToString())
                })
            };

            var verifyClient = _clientFactory.CreateClient();
            var verifyResponse = verifyClient.SendAsync(verifyRequest).Result;
            var verifyContent = verifyResponse.Content.ReadAsStringAsync().Result;

            if (!verifyResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation($"reCaptcha request failed. {verifyContent}");
                return false;
            }

            var responseJson = JToken.Parse(verifyContent);

            _logger.LogInformation(responseJson.ToString());

            if (!responseJson["success"].ToObject<bool>())
            {
                _logger.LogInformation("reCaptcha rejected. Result not true.");
                return false;
            }

            if (responseJson["score"].ToObject<float>() < 0.5)
            {
                _logger.LogInformation("reCaptcha rejected. Score too low.");
                return false;
            }

            if (responseJson["action"].ToString() != action)
            {
                _logger.LogInformation($"reCaptcha rejected. Action does not match {action}.");
                return false;
            }

            var timeDifference = DateTime.Now - DateTime.Parse(responseJson["challenge_ts"].ToString());

            if (timeDifference.Seconds > 60)
            {
                _logger.LogInformation($"reCaptcha rejected. Time duration too large. {timeDifference.Seconds}");
                return false;
            }

            if (responseJson["hostname"].ToString() != "forumx.sitict.net")
            {
                _logger.LogInformation("reCaptcha rejected. Hostname does not match.");
                return false;
            }

            return true;
        }
    }
}