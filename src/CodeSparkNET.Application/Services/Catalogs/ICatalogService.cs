using CodeSparkNET.Application.Dtos.Catalog;

namespace CodeSparkNET.Application.Services.Catalogs
{
    public interface ICatalogService
    {
        Task<List<CatalogNamesDto>> GetCatalogNamesAsync();
        Task<List<CatalogProductsDto>> GetCatalogProductsAsync(string catalogSlug);
        Task<CatalogProductDetailsDto> GetCatalogProductDetailsAsync(string catalogSlug, string productSlug);
        Task<CatalogDto> GetCatalogBySlugAsync(string catalogSlug);
    }
}