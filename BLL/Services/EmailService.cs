using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;

namespace BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendPasswordResetEmailAsync(
            string email,
            string username,
            string resetToken)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            var client = new SmtpClient(smtpSettings["Host"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]),
                EnableSsl = true
            };

            var resetLink = $"{_config["ClientUrl"]}/reset-password?token={resetToken}&email={email}";
            var body = $@"
                <h1>Password Reset</h1>
                <p>Hello {username},</p>
                <p>Please reset your password by clicking the link below:</p>
                <a href='{resetLink}'>{resetLink}</a>
                <p>The link will expire in 1 hour.</p>
            ";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"]),
                Subject = "Password Reset Request",
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);
            await client.SendMailAsync(mailMessage);
        }
    }
}
