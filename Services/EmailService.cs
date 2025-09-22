using CodeSparkNET.Interfaces.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace CodeSparkNET.Services
{
    /// <summary>
    /// Service responsible for sending emails such as account creation, 
    /// email confirmation and password reset using SMTP.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Sends an email to the user after account creation with a login link.
        /// </summary>
        public async Task SendAccountCratedEmailAsync(string toEmail, string firstName, string loginLink)
        {
            string html = $@"
            <html>
              <body style='font-family: Arial, sans-serif;'>
                <h2>Добро пожаловать, {System.Net.WebUtility.HtmlEncode(firstName)}!</h2>
                <p>Ваш аккаунт создан. Для входа перейдите по ссылке:</p>
                <p><a href='{loginLink}'>Войти в аккаунт</a></p>
                <p>Если вы не регистрировались — проигнорируйте это письмо.</p>
                <hr />
                <small>© Code Spark, {DateTime.UtcNow.Year}</small>
              </body>
            </html>";

            await SendEmailAsync(toEmail, "Регистрация CodeSpark", html, true);
        }

        /// <summary>
        /// Sends an email with a confirmation link to verify the user's email address.
        /// </summary>
        public Task SendEmailConfirmationAsync(string toEmail, string firstName, string confirmationLink)
        {
            string html = $@"
            <html><body style='font-family: Arial, sans-serif; background:#f4f6f8; margin:0; padding:20px;'>
              <div style='max-width:600px; margin:auto; background:#fff; padding:30px; border-radius:8px;'>
                <h2 style='color:#333;'>Подтверждение Email</h2>
                <p style='font-size:16px; color:#555;'>Привет, {firstName}!</p>
                <p style='font-size:16px; color:#555;'>Спасибо за регистрацию. Пожалуйста, подтвердите ваш email, нажав на кнопку ниже:</p>
                <p style='text-align:center;'>
                  <a href='{confirmationLink}' style='background:#0d6efd; color:#fff; padding:12px 24px; border-radius:6px; text-decoration:none; font-weight:bold;'>Подтвердить Email</a>
                </p>
                <p style='font-size:13px; color:#777;'>Если вы не регистрировались, просто проигнорируйте это письмо.</p>
                <p style='font-size:12px; color:#999; margin-top:30px;'>&copy; {DateTime.UtcNow.Year} Dot Net Tutorials. Все права защищены.</p>
              </div>
            </body></html>";
            return SendEmailAsync(toEmail, "Подтверждение Email", html, true);
        }

        /// <summary>
        /// Sends a reset password email with a secure reset link.
        /// </summary>
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

        /// <summary>
        /// Core method for sending an email using SMTP. 
        /// Handles connection, authentication, sending and exception logging.
        /// </summary>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage, bool isHtml = true)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_configuration["Smtp:From"]));
            msg.To.Add(MailboxAddress.Parse(toEmail));
            msg.Subject = subject;

            var body = new BodyBuilder();
            if (isHtml) body.HtmlBody = htmlMessage;
            msg.Body = body.ToMessageBody();

            try
            {
                // Attempt to send email
                using var client = new SmtpClient();
                await client.ConnectAsync(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"] ?? "587"));
                if (!string.IsNullOrEmpty(_configuration["Smtp:User"]))
                    await client.AuthenticateAsync(_configuration["Smtp:User"], _configuration["Smtp:Pass"]);

                await client.SendAsync(msg);
                await client.DisconnectAsync(true);

                // Success log
                _logger.LogInformation("Email successfully sent to {Email} with subject {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                // Error log
                _logger.LogError(ex, "Failed to send email to {Email} with subject {Subject}", toEmail, subject);
                return;
            }
        }
    }
}
