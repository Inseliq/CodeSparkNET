using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CodeSparkNET.Data;
using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;

namespace CodeSparkNET.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _catalogRepository;
        public CatalogService(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public async Task<List<CatalogNamesDto>> GetCatalogNamesAsync()
        {
            List<Catalog> catalogs = await _catalogRepository.GetCatalogsAsync();

            if (catalogs == null)
                return new List<CatalogNamesDto>();

            return catalogs
                    .Select(c => new CatalogNamesDto
                    {
                        Name = c.Name,
                        Slug = c.Slug
                    }).ToList();
        }

        public async Task<List<CatalogProductsDto>> GetCatalogProductsAsync(string catalogSlug)
        {
            var catalog = await _catalogRepository.GetCatalogBySlugAsync(catalogSlug);

            if (catalog == null || catalog.Products == null)
                return new List<CatalogProductsDto>();

            var catalogProducts = catalog.Products
                .Select(p => new CatalogProductsDto
                {
                    Name = p.Name,
                    Slug = p.Slug,
                    ShortDescription = p.ShortDescription,
                    Price = p.Price,
                    Currency = p.Currency,
                    InStock = p.InStock,
                    Image = p.ProductImages?.FirstOrDefault(img => img.IsMain)?.ImageData
                        ?? p.ProductImages?.FirstOrDefault()?.ImageData
                }).ToList();

            return catalogProducts;
        }

        public async Task<CatalogProductDetailsDto> GetCatalogProductDetailsAsync(string catalogSlug, string productSlug)
        {
            var catalog = await _catalogRepository.GetCatalogBySlugAsync(catalogSlug);

            var productDetails = catalog.Products.Find(p => p.Slug == productSlug);

            return new CatalogProductDetailsDto
            {
                Name = productDetails.Name,
                Slug = productDetails.Slug,
                FullDescription = productDetails.FullDescription,
                Price = productDetails.Price,
                Currency = productDetails.Currency,
                InStock = productDetails.InStock,
                Images = productDetails.ProductImages?
                    .Select(img => new CatalogProductImageDto
                    {
                        Name = img.Name,
                        ImageData = img.ImageData,
                        IsMain = img.IsMain
                    }).ToList()
            };
        }

        public async Task<CatalogDto> GetCatalogBySlugAsync(string catalogSlug)
        {
            var catalog = await _catalogRepository.GetCatalogBySlugAsync(catalogSlug);

            if (catalog == null)
                return new CatalogDto();

            return new CatalogDto
            {
                Name = catalog.Name,
                Slug = catalog.Slug,
                IsVisible = catalog.IsVisible,
                Products = catalog.Products
            };
        }
    }
}