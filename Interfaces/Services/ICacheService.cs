using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Dtos.Course;
using CodeSparkNET.Dtos.User;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Services
{
    public interface ICacheService
    {
        Task CacheCatalogNamesAsync();
        Task<List<CatalogNamesDto>> GetCachedCatalogNamesAsync();
        Task ClearCachedNamesAsync();
        Task CacheCatalogBySlugAsync(string slug);
        Task<CatalogDto> GetCachedCatalogBySlugAsync(string slug);
        Task ClearCachedCatalogBySlugAsync(string slug);
        Task CacheCourseBySlugAsync(string slug);
        Task<CourseDto> GetCachedCourseBySlugAsync(string slug);
        Task ClearCachedCourseBySlugAsync(string slug);
    }
}