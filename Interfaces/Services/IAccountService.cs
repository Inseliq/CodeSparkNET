using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeSparkNET.Interfaces.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto, string loginLink);
        Task<SignInResult> PasswordSignInAsync(LoginDto dto);
        Task SignOutAsync();
        Task<IList<string>> GetRolesAsync(AppUser user);
        Task<bool> SendPasswordResetLinkAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model);
        Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto model);
        Task<AppUser> GetUserAsync(ClaimsPrincipal user);
        Task<bool> UserWithEmailExistsAsync(string email);
        Task<bool> UserWithUserNameExistsAsync(string userName);
        Task<bool> AddCourseToUserAsync(AppUser user, string courseSlug);
        Task<bool> IsCourseAlreadyEnrolled(AppUser user, string courseSlug);
    }
}