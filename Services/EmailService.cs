using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Interfaces;

namespace CodeSparkNET.Services
{
    public class EmailService : IEmailService
    {
        public Task SendAccountCratedEmailAsync(string toEmail, string firstEmail, string loginLink)
        {
            throw new NotImplementedException();
        }

        public Task SendResetPasswordEmailAsync(string toEmail, string firstName, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}