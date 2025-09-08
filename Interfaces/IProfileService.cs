
using System.Security.Claims;
using CodeSparkNET.Dtos.Profile;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Interfaces
{
    public interface IProfileService
    {
        Task<IdentityResult> UpdatePersonalProfileAsync(string email, UpdatePersonalProfileDto personalProfile);
        Task<bool> SendEmailConfirmationLinkAsync(string email);
        Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model);
        // Task UpdateUserClaims(ClaimsPrincipal claimsPrincipal);

    }
}