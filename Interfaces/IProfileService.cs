
using CodeSparkNET.Dtos.Profile;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Interfaces
{
    public interface IProfileService
    {
        Task<PersonalProfileDto> GetPersonalInfoAsync(string email);
        Task<IdentityResult> UpdatePersonalProfileAsync(string email, UpdatePersonalProfileDto personalProfile);
        Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model);
    }
}