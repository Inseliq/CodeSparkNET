using CodeSparkNET.Application.Dtos.Catalog;
using CodeSparkNET.Application.Dtos.Course;

namespace CodeSparkNET.Application.Services.Cache
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