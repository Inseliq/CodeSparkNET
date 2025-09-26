
using System.Security.Claims;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Interfaces.Services
{
    public interface IProfileService
    {
        Task<IdentityResult> UpdatePersonalProfileAsync(string email, UpdatePersonalProfileDto personalProfile);
        Task<bool> SendEmailConfirmationLinkAsync(string email);
        Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model);
        Task UpdateUserClaims(AppUser user);
        Task<List<AllUserCoursesDto>> GetAllUserCoursesAsync(AppUser user);
    }
}