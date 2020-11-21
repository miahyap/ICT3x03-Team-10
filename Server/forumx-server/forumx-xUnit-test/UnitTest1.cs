using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using forumx_server;
using forumx_server.Captcha;
using forumx_server.Database;
using forumx_server.Email;
using forumx_server.Helper;
using forumx_server.Model;
using forumx_server.OauthVerifier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using OtpNet;
using Xunit;
using Xunit.Sdk;

namespace forumx_xUnit_test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal("test", "test");
        }
    }

    public class MockDatabase : IDatabase
    {
        public void AddUser(User user)
        {
        }

        public void ChangePassword(User user)
        {
            throw new NotImplementedException();
        }

        public bool CheckEmailExists(string email)
        {
            return false;
        }

        public bool CreateComment(Comment comment, User user)
        {
            return true;
        }

        public bool CreatePost(Post post, User user)
        {
            return true;
        }

        public bool DeleteComment(Comment comment, User user)
        {
            return true;
        }

        public bool VerifyPostUser(User user, Post post)
        {
            return true;
        }

        public bool VerifyCommentUser(User user, Comment comment)
        {
            return true;
        }

        public bool DeletePost(Post post, User user)
        {
            return true;
        }

        public List<ActivityLog> GetActivityLog(User user)
        {
            throw new NotImplementedException();
        }

        public List<Post> SearchPost(string searchTerm)
        {
            return new List<Post>();
        }

        public List<Comment> GetCommentsByPost(Guid postUuid)
        {
            return new List<Comment> {new Comment()};
        }

        public List<Post> GetPostByTopic(Guid topicUuid)
        {
            return new List<Post> {new Post()};
        }

        public Post GetPostInfo(Guid postGuid)
        {
            return new Post();
        }

        public List<Topic> GetTopics(string topicUuid)
        {
            return new List<Topic> {new Topic()};
        }

        public User GetUser(string username)
        {
            if (username == "test123")
            {
                //test login api
                var testUser = new User {Username = "test123"};
                var passwordHash =
                    "37250026DFF8BD29FA5551A92389D31E0175D6264D66D8489CE98942146F9103EA35BCE54480C251BB9288BB6E90039F"; //test123
                testUser.PasswordHash = Enumerable.Range(0, passwordHash.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(passwordHash.Substring(x, 2), 16))
                    .ToArray();

                return testUser;
            }

            return null;
        }

        public User GetUser(Guid userGuid)
        {
            throw new NotImplementedException();
        }

        public bool LogActivity(IPAddress ipAddress, string activity, User user)
        {
            return true;
        }

        public bool UpdateComment(Comment comment, User user)
        {
            return true;
        }

        public bool UpdatePost(Post post, User user)
        {
            return true;
        }
    }

    public class MockOauthVerifier : IOauthProvider
    {
        public User VerifyUserFromOauthToken(string token, OauthActionEnum actionEnum)
        {
            if (token != "pass") return null;

            return new User
            {
                Name = "testuser123",
                Email = "test"
            };
        }
    }

    public class MockEmailSender : IEmailSender
    {
        public void SendChangePasswordEmail(User user)
        {
        }

        public void SendRegisterEmail(User user)
        {
        }
    }

    public class MockCaptcha : ICaptcha
    {
        public bool VerifyCaptcha(string captchaResponse, IPAddress ipAddress, string action)
        {
            if (captchaResponse == "pass")
            {
                return captchaResponse == "pass";
            }

            return false;
        }
    }

    public class BaseControllerTests
    {
        protected readonly HttpClient DevClient;
        protected readonly TestServer DevServer;
        protected readonly HttpClient RelClient;
        protected readonly TestServer RelServer;

        public BaseControllerTests()
        {
            var appPath = PlatformServices.Default.Application.ApplicationBasePath;
            var binPosition = appPath.IndexOf(Path.DirectorySeparatorChar + "bin", StringComparison.Ordinal);
            var basePath = appPath.Remove(binPosition);

            var backslashPosition =
                basePath.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
            basePath = basePath.Remove(backslashPosition);
            basePath = Path.Join(basePath, "forumx-server");
            Directory.SetCurrentDirectory(basePath);

            var devBuilder = new WebHostBuilder()
                .UseContentRoot(basePath)
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile(Path.Join(basePath, "appsettings.json"))
                    .Build()
                ).UseStartup<Startup>()
                .UseEnvironment("Development")
                .ConfigureTestServices(services =>
                {
                    services.AddScoped<IDatabase, MockDatabase>();
                    services.AddScoped<IOauthProvider, MockOauthVerifier>();
                    services.AddScoped<IEmailSender, MockEmailSender>();
                    services.AddScoped<ICaptcha, MockCaptcha>();
                });

            DevServer = new TestServer(devBuilder);
            DevClient = DevServer.CreateClient();

            var relBuilder = new WebHostBuilder()
                .UseContentRoot(basePath)
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile(Path.Join(basePath, "appsettings.json"))
                    .Build()
                ).UseStartup<Startup>().UseEnvironment("Production");

            RelServer = new TestServer(relBuilder);
            RelClient = RelServer.CreateClient();
        }

        public string GetToken()
        {
            var tokenResponseMessage = DevClient.PostAsync("/api/Account/RegisterTest", null).Result;
            var tokenJson = tokenResponseMessage.Content.ReadAsStringAsync().Result;
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["token"];
            return token;
        }
    }

    public class DisplayTestMethodNameAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            Console.WriteLine("Setup for test '{0}.'", methodUnderTest.Name);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            Console.WriteLine("TearDown for test '{0}.'", methodUnderTest.Name);
        }
    }


    public class AuthControllerTests : BaseControllerTests
    {
        [Fact]
        public async Task TestAuthRequired()
        {
            var response = await DevClient.GetAsync("/api/Account/CredsTest");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TestAuthSuccess()
        {
            var token = GetToken();
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Account/CredsTest");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestAuthFailure()
        {
            var token = GetToken();
            token = token + "a";
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Account/CredsTest");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TestClientRateLimiting()
        {
            var tokenOne = GetToken();
            var tokenTwo = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Account/CredsTest"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Account/CredsTest"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Account/CredsTest"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenTwo);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        //test topic list rate limit rules
        [Fact]
        public async Task TestGetTopicListRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Topic/"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Topic/"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Topic/"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetTopicRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Topic/54bd9c06-6a6f-8e4f-b5cf-ebb98db054c5"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Topic/54bd9c06-6a6f-8e4f-b5cf-ebb98db054c5"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Topic/54bd9c06-6a6f-8e4f-b5cf-ebb98db054c5"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestSessionRevoke()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Account/CredsTest"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/TerminateSession"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Account/CredsTest"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestJwtGeneratorInactive()
        {
            var tokenResponseMessage = await RelClient.PostAsync("/api/Account/RegisterTest", null);
            Assert.Equal(HttpStatusCode.NotFound, tokenResponseMessage.StatusCode);
        }

        //Test Login Auth
        [Fact]
        public async Task TestLoginAuthSuccess()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new {username = "test123", password = "test123", captcha = "pass"}),
                    Encoding.UTF8, "application/json")
            };
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestLoginAuthFailure()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new {username = "test123", password = "randomstuff", captcha = "pass"}),
                    Encoding.UTF8,
                    "application/json")
            };
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TestLoginAuthFailureResponseEqual()

        {
            using var wrongUserRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new {username = "test1234", password = "test123", captcha = "pass"}),
                    Encoding.UTF8,
                    "application/json")
            };
            var wrongUserResponse = await DevClient.SendAsync(wrongUserRequestMessage);

            using var wrongPasswordRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new {username = "test123", password = "test1234", captcha = "pass"}),
                    Encoding.UTF8,
                    "application/json")
            };
            var wrongPasswordResponse = await DevClient.SendAsync(wrongPasswordRequestMessage);

            var wrongUserResponseContent =
                JsonSerializer.Deserialize<Dictionary<string, object>>(wrongUserResponse.Content.ReadAsStringAsync()
                    .Result);
            var wrongPasswordResponseContent =
                JsonSerializer.Deserialize<Dictionary<string, object>>(wrongPasswordResponse.Content.ReadAsStringAsync()
                    .Result);

            wrongUserResponseContent.Remove("traceId");
            wrongPasswordResponseContent.Remove("traceId");

            Assert.True(wrongPasswordResponseContent.Intersect(wrongPasswordResponseContent).Count() ==
                        wrongPasswordResponseContent.Union(wrongPasswordResponseContent).Count());
            Assert.Equal(wrongUserResponse.StatusCode, wrongPasswordResponse.StatusCode);
        }

        [Fact]
        public async Task TestLoginUsernameExceedLength()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        username = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghija", password = "test123",
                        captcha = "pass"
                    }),
                    Encoding.UTF8, "application/json")
            };
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TestLoginPasswordExceedLength()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        username = "test123",
                        password = "test123456test123456test123456test123456test123456test123456test123456",
                        captcha = "pass"
                    }),
                    Encoding.UTF8, "application/json")
            };
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TestLoginCaptchaSuccess()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content =
                    new StringContent(
                        JsonSerializer.Serialize(new {username = "test123", password = "test123", captcha = "pass"}),
                        Encoding.UTF8, "application/json")
            };
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestLoginCaptchaFailure()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content =
                    new StringContent(
                        JsonSerializer.Serialize(new {username = "test123", password = "test123", captcha = "fail"}),
                        Encoding.UTF8, "application/json")
            };
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task TestRegisterSuccess()
        {
            string registerCode;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/GenerateOTP"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = registerCode}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/RegisterAccount"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                        {
                            token = registerCode, username = "testuser123", password = "testuser123", captcha = "pass"
                        }), Encoding.UTF8,
                        "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestRegisterTotpNotGenerated()
        {
            string registerCode;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/RegisterAccount"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                        {
                            token = registerCode, username = "testuser123", password = "testuser123", captcha = "pass"
                        }), Encoding.UTF8,
                        "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestRegisterUsernameFail()
        {
            string registerCode;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/GenerateOTP"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = registerCode}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/RegisterAccount"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                            {token = registerCode, username = "test", password = "testuser123", captcha = "pass"}),
                        Encoding.UTF8,
                        "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestRegisterWeakPassword()
        {
            string registerCode;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckPassword"))
            {
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(new {token = registerCode, data = "1234567"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                var passwordCheck = JsonSerializer.Deserialize<Dictionary<string, bool>>(tokenJson)["status"];
                Assert.False(passwordCheck);
            }

            Thread.Sleep(1000);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckPassword"))
            {
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(new {token = registerCode, data = "password"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                var passwordCheck = JsonSerializer.Deserialize<Dictionary<string, bool>>(tokenJson)["status"];
                Assert.False(passwordCheck);
            }
        }

        [Fact]
        public async Task TestRegisterPasswordInCommonList()
        {
            string registerCode;


            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/GenerateOTP"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = registerCode}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            //check password in common list
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/RegisterAccount"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                            {token = registerCode, username = "testuser123", password = "password", captcha = "pass"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestRegisterPasswordExceedLength()
        {
            string registerCode;


            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/GenerateOTP"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = registerCode}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            //check password in common list
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/RegisterAccount"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                        {
                            token = registerCode,
                            username = "testuser123",
                            password = "1234567890123456789012345678901234567890123456789012345678901234567890",
                            captcha = "pass"
                        }),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestRegisterPasswordIsNull()
        {
            string registerCode;


            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/GenerateOTP"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = registerCode}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            //check password in common list
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/RegisterAccount"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                            {token = registerCode, username = "testuser123", password = "", captcha = "pass"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }


        [Fact]
        public async Task TestRegisterOauthFail()
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity");
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "fail"}),
                Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestCheckUsernameCaptchaFail()
        {
            string registerCode;


            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckUsername"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new {token = registerCode, data = "test", captcha = "fail"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestCheckUsernameTokenFail()
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckUsername"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                            {token = "3cd14ca9-9178-43cd-b03d-c64c5dcf6c3b", data = "test", captcha = "pass"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestCheckUsernameEmptyFields()
        {
            string registerCode;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }


            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckUsername"))
            {
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(new {data = "test", captcha = "pass"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckUsername"))
            {
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(new {token = registerCode, data = "test"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestCheckUsernameRateLimiting()
        {
            string registerCode;


            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckUsername"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new {token = registerCode, data = "test", captcha = "pass"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckUsername"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new {token = registerCode, data = "test", captcha = "pass"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestRegisterCaptchaFail()
        {
            string registerCode;
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/VerifyIdentity"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = "pass"}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                registerCode = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenJson)["registerCode"];
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            //check username requirements
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckUsername"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new {token = registerCode, data = "testuser123", captcha = "pass"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                var usernameCheck = JsonSerializer.Deserialize<Dictionary<string, bool>>(tokenJson)["status"];
                Assert.True(usernameCheck);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/CheckPassword"))
            {
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(new {token = registerCode, data = "testuser123"}),
                        Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson = await response.Content.ReadAsStringAsync();
                var passwordCheck = JsonSerializer.Deserialize<Dictionary<string, bool>>(tokenJson)["status"];
                Assert.True(passwordCheck);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/GenerateOTP"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new {token = registerCode}),
                    Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/RegisterAccount"))
            {
                requestMessage.Content =
                    new StringContent(
                        JsonSerializer.Serialize(new
                        {
                            token = registerCode, username = "testuser123", password = "testuser123", captcha = "fail"
                        }), Encoding.UTF8,
                        "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }


    public class PostControllerTest : BaseControllerTests
    {
        [Fact]
        public async Task TestGetPostCommentPass()
        {
            var tokenOne = GetToken();
            using var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestGetPostAuthFail()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            tokenOne = tokenOne + "a";

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetPostCommentsRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestGetPostInvalidInput()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/Post/qqq"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestNewPostPass()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                title = "new test title!",
                content = "hey new test post!",
                captcha = "pass"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestNewPostRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    title = "new test title!",
                    content = "hey new test post!",
                    captcha = "pass"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    title = "new test title!",
                    content = "hey new test post!",
                    captcha = "pass"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestNewPostInvalidInput()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                title = "new test title!",
                content = "hey new test post!"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestNewPostCaptchaFail()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                title = "new test title!",
                content = "hey new test post!",
                captcha = "fail"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestNewPostLengthLimit()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                title = "123456789012345678901234567890123456789012345678901",
                content = "hey new test post!",
                captcha = "pass"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestUpdatePostPass()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdatePostRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdatePostInvalidUuid()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed",
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdatePostEmptyUuid()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }


        [Fact]
        public async Task TestDeletePostPass()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestDeletePostInvalidUuid()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, "/api/Post/f1e170d5-6057-4219-92eb"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }


        [Fact]
        public async Task TestDeletePostRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }


        [Fact]
        public async Task TestNewPost()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    title = "new test title!",
                    content = "hey new test post!",
                    captcha = "pass"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Post/NewPost"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "UnauthorizedUser");
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    topic = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    title = "new test title!",
                    content = "hey new test post!",
                    captcha = "pass"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdatePost()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Post"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized User");
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test post!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestDeletePost()
        {
            var tokenOne = GetToken();

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, "/api/Post/f1e170d5-6057-4219-92eb-5bef70a46e6d"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "UnauthorizedUser");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }
    }


    public class CommentControllerTest : BaseControllerTests
    {
        [Fact]
        public async Task TestNewComment()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    post = "f1e170d5-6057-4219-92eb-5bef70a46e6d",
                    content = "hey new test comment!",
                    captcha = "pass"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "UnauthorizedUser");
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    post = "f1e170d5-6057-4219-92eb-5bef70a46e6d",
                    content = "hey new test comment!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestNewCommentInvalidInput()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                post = "f1e170d5-6057-4219-92eb-5bef70a46e6d",
                content = "hey new test comment!"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestNewCommentCaptchaFail()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                post = "f1e170d5-6057-4219-92eb-5bef70a46e6d",
                content = "hey new test comment!",
                captcha = "fail"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestNewCommentLengthLimit()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                post = "f1e170d5-6057-4219-92eb-5bef70a46e6d",
                content = "lJNtGz1gqic434LIuOWraebsvU72igAVklwt1YBOz7vF3AdHJNqjV3SInqWlyziY2sjGqjwTTefCxNjZ7gBnL504xqbCLayLHSenpuJG96aRqjCXgEBsczuZvaljojED1",
                captcha = "pass"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [Fact]
        public async Task TestNewCommentInvalidUuid()
        {
            var tokenOne = GetToken();

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
            {
                post = "abc",
                content = "hey new test comment!",
                captcha = "pass"
            }), Encoding.UTF8, "application/json");
            var response = await DevClient.SendAsync(requestMessage);
            var tokenJson1 = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestNewCommentRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    post = "f1e170d5-6057-4219-92eb-5bef70a46e6d",
                    content = "hey new test post!",
                    captcha = "pass"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                var tokenJson1 = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Comment/NewComment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    post = "f1e170d5-6057-4219-92eb-5bef70a46e6d",
                    content = "hey new test post!",
                    captcha = "pass"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdateCommentSuccess()
        {
            var tokenOne = GetToken();
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Comment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test comment!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
        
        [Fact]
        public async Task TestUpdateCommentUnauthorized()
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Comment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized User");
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test comment!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdateCommentInvalidInput()
        {
            var tokenOne = GetToken();
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Comment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = ""
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdateCommentInvalidUuid()
        {
            var tokenOne = GetToken();
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Comment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "abc",
                    content = "hey new test comment!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestUpdateCommentRateLimitRule()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Comment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test comment!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Comment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test comment!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/api/Comment"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    uuid = "b440b361-e3b1-e348-88ed-e8c74c13ede7",
                    content = "hey new test comment!"
                }), Encoding.UTF8, "application/json");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }


        [Fact]
        public async Task TestDeleteComment()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/Comment/aa1aae76-b3ab-4a39-9436-d4b7e1ce2951"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/Comment/aa1aae76-b3ab-4a39-9436-d4b7e1ce2951"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "UnauthorizedUser");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestDeleteCommentInvalidUuid()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/api/Comment/abc"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }

    public class SearchControllerTest : BaseControllerTests
    {
        [Fact]
        public async Task TestSearchSuccess()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Search/aaaaa"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Headers.Add("Captcha", "pass");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestSearchUnauthorized()
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Search/aaaaa"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "UnauthorizedUser");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task TestSearchInvalidCaptcha()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Search/aaaaa"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Headers.Add("Captcha", "fail");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        //[Fact]
        public async Task TestSearchMissingCaptchaHeader()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Search/aaaaa"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        //[Fact]
        public async Task TestSearchInputLessThan5Characters()
        {
            var tokenOne = GetToken();

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/Search/aaaa"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOne);
                requestMessage.Headers.Add("Captcha", "pass");
                var response = await DevClient.SendAsync(requestMessage);
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }


    public class HelperTest
    {
        [Fact]
        public void Pbkdf2PasswordImplementationTest()
        {
            //test string to be supplied into the method
            var test = "test123";

            var hash = Pbkdf2Password.PasswordToHash(test);

            //Method returns true if password match using the same derivation function
            var result = Pbkdf2Password.CheckPasswordHash(hash, test);

            //Implementation is incorrect if its false.
            Assert.True(result, "Implementation issue");
        }

        [Fact]
        public void Pbkdf2PasswordMismatchTestValidity()
        {
            //test string to be supplied into the method
            var test = "test";
            var test2 = "test2";

            var hash = Pbkdf2Password.PasswordToHash(test);

            //Method returns false if password mismatch using the same derivation function
            var result = Pbkdf2Password.CheckPasswordHash(hash, test2);

            Assert.False(result, "Password matches");
        }

        [Fact]
        public void AesGcmEncryptTest()
        {
            var key = new byte[32];
            new RNGCryptoServiceProvider().GetBytes(key);
            var plaintext = "test123321BlabBlahBlah";
            var ciphertest = CryptoConfigHelper.Encrypt(plaintext, key);
            var decrypted = CryptoConfigHelper.Decrypt(ciphertest, key);
            Assert.Equal(plaintext, decrypted);
        }

        [Fact]
        public void TotpTest()
        {
            var key = new byte[32];
            RandomNumberGenerator.Fill(key);
            OtpHelper.GenerateTotp(key);
            var otp = new Totp(key, mode: OtpHashMode.Sha1, totpSize: 6);
            var result = otp.ComputeTotp();
            Assert.True(OtpHelper.VerifyOtp(key, result));
        }
    }

    public class XmlTest : BaseControllerTests
    {
        [Fact]
        public async Task TestXmlNotSupported()
        {
            var xmlString =
                "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>< root >< captcha > pass </ captcha >< password > test123 </ password >< username > test123 </ username ></ root >";
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Login")
            {
                Content = new StringContent(
                    xmlString,
                    Encoding.UTF8, "application/xml")
            };
            var response = await DevClient.SendAsync(requestMessage);
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
        }
    }
}