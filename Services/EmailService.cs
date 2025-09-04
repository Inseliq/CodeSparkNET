using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;

namespace CodeSparkNET.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendAccountCratedEmailAsync(string toEmail, string firstEmail, string loginLink)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, bool isHtml = true)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_configuration["Smtp:From"]));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;

            var body = new BodyBuilder();
            if (isHtml) body.HtmlBody = htmlMessage;

            msg.Body = body.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"] ?? "587"));
            if (!string.IsNullOrEmpty(_configuration["Smtp:User"]))
                await client.AuthenticateAsync(_configuration["Smtp:User"], _configuration["Smtp:Pass"]);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string firstName, string resetLink)
        {
            string html = $@"
            <html><body style='font-family: Arial, sans-serif; background:#f4f6f8; margin:0; padding:20px;'>
              <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                <h2 style='color:#333;'>Password Reset Request</h2>
                <p style='font-size:16px; color:#555;'>Hi {firstName},</p>
                <p style='font-size:16px; color:#555;'>We received a request to reset your password. Click the button below to choose a new one.</p>
                <p style='text-align:center;'>
                  <a href='{resetLink}' style='background:#0d6efd; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Reset Password</a>
                </p>
                <p style='font-size:13px; color:#777;'>If you didn't request this, you can ignore this email.</p>
                <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} Dot Net Tutorials. All rights reserved.</p>
              </div>
            </body></html>";
            await SendEmailAsync(toEmail, "Восстановление пароля", html, true);
        }
    }
}