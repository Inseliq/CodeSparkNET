using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Account;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto, string loginLink);
        Task<SignInResult> PasswordSignInAsync(LoginDto dto);
        Task SignOutAsync();
        Task<bool> SendPasswordResetLinkAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}