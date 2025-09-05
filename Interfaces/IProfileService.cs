
using CodeSparkNET.Dtos.Profile;
using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Interfaces
{
    public interface IProfileService
    {
        Task<IdentityResult> UpdatePersonalProfileAsync(string email, UpdatePersonalProfileDto personalProfile);
    }
}