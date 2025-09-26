using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Services
{
    public interface ICatalogService
    {
        Task<List<CatalogNamesDto>> GetCatalogNamesAsync();
        Task<List<CatalogProductsDto>> GetCatalogProductsAsync(string catalogSlug);
        Task<CatalogProductDetailsDto> GetCatalogProductDetailsAsync(string catalogSlug, string productSlug);
        Task<CatalogDto> GetCatalogBySlugAsync(string catalogSlug);
    }
}