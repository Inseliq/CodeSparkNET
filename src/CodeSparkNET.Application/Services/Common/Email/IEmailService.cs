namespace CodeSparkNET.Application.Services.Common.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage, bool isHtml = true);
        Task SendAccountCratedEmailAsync(string toEmail, string firstEmail, string loginLink);
        Task SendEmailConfirmationAsync(string toEmail, string firstName, string confirmationLink);
        Task SendResetPasswordEmailAsync(string toEmail, string firstName, string resetLink);
    }
}