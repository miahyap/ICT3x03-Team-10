using forumx_server.Model;

namespace forumx_server.OauthVerifier
{
    public enum OauthActionEnum
    {
        Register,
        Reset
    }

    public interface IOauthProvider
    {
        public User VerifyUserFromOauthToken(string token, OauthActionEnum actionEnum);
    }
}