using CodeSparkNET.Application.Dtos.Account;
using CodeSparkNET.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CodeSparkNET.Application.Services.User
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto, string loginLink);
        Task<SignInResult> PasswordSignInAsync(LoginDto dto);
        Task SignOutAsync();
        Task RefrashSignInAsync(AppUser user);
        Task<IList<string>> GetRolesAsync(AppUser user);
        Task<bool> SendPasswordResetLinkAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model);
        Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto model);
        Task<AppUser> GetUserAsync(ClaimsPrincipal user);
        Task<bool> UserWithEmailExistsAsync(string email);
        Task<bool> UserWithUserNameExistsAsync(string userName);
        Task<bool> AddCourseToUserAsync(string userId, string courseSlug);
        Task<bool> IsCourseAlreadyEnrolled(string userId, string courseSlug);
    }
}