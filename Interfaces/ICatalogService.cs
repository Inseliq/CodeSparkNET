using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Catalog;

namespace CodeSparkNET.Interfaces
{
    public interface ICatalogService
    {
        Task<List<CatalogNamesDto>> GetCatalogNamesAsync();
        Task<List<CatalogProductsDto>> GetCatalogProductsAsync(string catalogSlug);
        Task<CatalogProductDetailsDto> GetCatalogProductDetailsAsync(string catalogSlug, string productSlug);
        Task<CatalogDto> GetCatalogBySlugAsync(string catalogSlug);
    }
}