using CodeSparkNET.Domain.Models;

namespace CodeSparkNET.Application.Services
{
    public interface ICatalogRepository
    {
        Task<List<Catalog>> GetCatalogsAsync();
        Task<Catalog> GetCatalogBySlugAsync(string catalogSlug);
    }
}