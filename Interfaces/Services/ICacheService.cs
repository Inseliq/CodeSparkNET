using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Services
{
    public interface ICacheService
    {
        Task CacheUserAsync(string email);
        Task<AppUser> GetCachedUserAsync(string email);
        Task ClearCachedUserAsync(string email);
        Task CacheCatalogsAsync();
        Task<List<Catalog>> GetCachedCatalogsAsync();
    }
}