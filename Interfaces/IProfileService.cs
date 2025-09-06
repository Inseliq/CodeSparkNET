
using System.Security.Claims;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Interfaces
{
    public interface IProfileService
    {
        Task<IdentityResult> UpdatePersonalProfileAsync(string email, UpdatePersonalProfileDto personalProfile);
        Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model);
        Task<bool> SendEmailConfirmationLinkAsync(string email);
        Task<AppUser> GetUserAsync(ClaimsPrincipal user);
        Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto model);
        Task RefreshSignInAsync(AppUser user);
    }
}