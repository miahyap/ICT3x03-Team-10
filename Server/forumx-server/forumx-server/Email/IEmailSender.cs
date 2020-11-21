using forumx_server.Model;

namespace forumx_server.Email
{
    public interface IEmailSender
    {
        public void SendRegisterEmail(User user);
        public void SendChangePasswordEmail(User user);
    }
}