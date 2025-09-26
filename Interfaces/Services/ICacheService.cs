using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Dtos.User;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Services
{
    public interface ICacheService
    {
        Task CacheUserAsync(string email);
        Task<UserDto> GetCachedUserAsync(string email);
        Task ClearCachedUserAsync(string email);
        Task CacheCatalogNamesAsync();
        Task<List<CatalogNamesDto>> GetCachedCatalogNamesAsync();
        Task ClearCachedNamesAsync();
        Task CacheCatalogBySlugAsync(string slug);
        Task<Catalog> GetCachedCatalogBySlugAsync(string slug);
        Task ClearCachedCatalogBySlugAsync(string slug);
    }
}