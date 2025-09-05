
using CodeSparkNET.Dtos.Profile;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Interfaces
{
    public interface IProfileService
    {
        Task<PersonalProfileDto> GetPersonalInfoAsync(string email);
        Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model);
    }
}