using CodeSparkNET.Dtos.User;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByEmailAsync(string email);
    }
}
