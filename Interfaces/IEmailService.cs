using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage, bool isHtml = true);
        Task SendAccountCratedEmailAsync(string toEmail, string firstEmail, string loginLink);
        Task SendEmailConfirmationAsync(string toEmail, string firstName, string confirmationLink);
        Task SendResetPasswordEmailAsync(string toEmail, string firstName, string resetLink);
    }
}