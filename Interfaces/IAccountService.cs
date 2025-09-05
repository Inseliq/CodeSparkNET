using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeSparkNET.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto, string loginLink);
        Task<SignInResult> PasswordSignInAsync(LoginDto dto);
        Task SignOutAsync();
        Task<bool> SendPasswordResetLinkAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model);
        Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model);
        Task<bool> SendEmailConfirmationLinkAsync(string email);
        Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto model);
        Task<AppUser> GetUserAsync(ClaimsPrincipal user);
    }
}