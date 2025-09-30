using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.User;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CodeSparkNET.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByEmailAsync(string email);
        Task<AppUser> GetUserByUserNameAsync(string userName);
        Task<AppUser> GetUserAsync(ClaimsPrincipal user);
        Task<IdentityResult> CreateUserAsync(AppUser user, string password);
        Task<IdentityResult> UpdateUserAsync(AppUser user);
        Task<IdentityResult> DeleteUserAsync(AppUser user);
        Task<IdentityResult> AddToRoleAsync(AppUser user, string role);
        Task<IList<string>> GetUserRolesAsync(AppUser user);
        Task<bool> IsEmailConfirmedAsync(AppUser user);
        Task<string> GeneratePasswordResetTokenAsync(AppUser user);
        Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string newPassword);
        Task UpdateSecurityStampAsync(AppUser user);
        Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(AppUser user);
        Task<SignInResult> PasswordSignInAsync(LoginDto model);
        Task RefreshSignInAsync(AppUser user);
        Task SignOutAsync();
    }
}
