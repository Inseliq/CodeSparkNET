using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces
{
    public interface IEmailService
    {
        Task SendAccountCratedEmailAsync(string toEmail, string firstEmail, string loginLink);
        Task SendResetPasswordEmailAsync(string toEmail, string firstName, string resetLink);
    }
}