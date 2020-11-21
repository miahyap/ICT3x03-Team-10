using System.IO;
using forumx_server.Model;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace forumx_server.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string _apiKey;
        private readonly string _changePassTemplate;
        private readonly string _registerTemplate;

        public SendGridEmailSender(IConfiguration configuration)
        {
            _apiKey = configuration["SendGrid:APIKey"];
            _registerTemplate = configuration["SendGrid:RegisterTemplate"];
            _changePassTemplate = configuration["SendGrid:ChangePassTemplate"];
        }

        public void SendRegisterEmail(User user)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("no-reply@forumx.natchan.dev", "Forumx Notification");
            var to = new EmailAddress(user.Email, user.Name);
            var subject = "Forumx account created";
            var message =
                $"Hello {user.Name},\r\n\r\nYour forumx account was created.\r\n\r\nWe Hope you will enjoy using our site!";
            var htmlMessage = File.ReadAllText(_registerTemplate);
            htmlMessage = htmlMessage.Replace("{name}", user.Name);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, htmlMessage);
            client.SendEmailAsync(msg);
        }

        public void SendChangePasswordEmail(User user)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("no-reply@forumx.natchan.dev", "Forumx Notification");
            var to = new EmailAddress(user.Email, user.Name);
            var subject = "Forumx account password changed";
            var message =
                $"Hello {user.Name},\r\n\r\nYour forumx account password was changed.\r\n\r\nWe Hope you will enjoy using our site!";
            var htmlMessage = File.ReadAllText(_changePassTemplate);
            htmlMessage = htmlMessage.Replace("{name}", user.Name);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, htmlMessage);
            client.SendEmailAsync(msg);
        }
    }
}