using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Interfaces.Repositories;
using CodeSparkNET.Interfaces.Services;

namespace CodeSparkNET.Services
{
    /// <summary>
    /// Service for handling catalog-related operations such as retrieving catalog names, products, and details.
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly ILogger<CatalogService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogService"/> class.
        /// </summary>
        /// <param name="catalogRepository">Repository for accessing catalog data.</param>
        /// <param name="logger">Logger for logging errors and information.</param>
        public CatalogService(ICatalogRepository catalogRepository, ILogger<CatalogService> logger)
        {
            _catalogRepository = catalogRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of catalog names with their slugs.
        /// </summary>
        /// <returns>A list of <see cref="CatalogNamesDto"/>.</returns>
        public async Task<List<CatalogNamesDto>> GetCatalogNamesAsync()
        {
            try
            {
                var catalogs = await _catalogRepository.GetCatalogsAsync();

                if (catalogs == null)
                    return new List<CatalogNamesDto>();

                return catalogs
                        .Select(c => new CatalogNamesDto
                        {
                            Name = c.Name,
                            Slug = c.Slug
                        }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving catalog names.");
                return new List<CatalogNamesDto>();
            }
        }

        /// <summary>
        /// Retrieves a list of products for a given catalog slug.
        /// </summary>
        /// <param name="catalogSlug">The slug of the catalog.</param>
        /// <returns>A list of <see cref="CatalogProductsDto"/>.</returns>
        public async Task<List<CatalogProductsDto>> GetCatalogProductsAsync(string catalogSlug)
        {
            try
            {
                var catalog = await _catalogRepository.GetCatalogBySlugAsync(catalogSlug);

                if (catalog == null || catalog.Products == null)
                    return new List<CatalogProductsDto>();

                return catalog.Products
                    .Select(p => new CatalogProductsDto
                    {
                        Name = p.Name,
                        Slug = p.Slug,
                        ShortDescription = p.ShortDescription,
                        Price = p.Price,
                        Currency = p.Currency,
                        InStock = p.InStock,
                        ProductType = p.ProductType,
                        Image = p.ProductImages?.FirstOrDefault(img => img.IsMain)?.ImageData
                            ?? p.ProductImages?.FirstOrDefault()?.ImageData
                    }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving products for catalog slug {catalogSlug}.", catalogSlug);
                return new List<CatalogProductsDto>();
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific product in a catalog.
        /// </summary>
        /// <param name="catalogSlug">The slug of the catalog.</param>
        /// <param name="productSlug">The slug of the product.</param>
        /// <returns>A <see cref="CatalogProductDetailsDto"/> containing product details.</returns>
        public async Task<CatalogProductDetailsDto> GetCatalogProductDetailsAsync(string catalogSlug, string productSlug)
        {
            try
            {
                var catalog = await _catalogRepository.GetCatalogBySlugAsync(catalogSlug);
                var productDetails = catalog?.Products?.Find(p => p.Slug == productSlug);

                if (productDetails == null)
                    return null;

                return new CatalogProductDetailsDto
                {
                    Name = productDetails.Name,
                    Slug = productDetails.Slug,
                    FullDescription = productDetails.FullDescription,
                    Price = productDetails.Price,
                    Currency = productDetails.Currency,
                    InStock = productDetails.InStock,
                    ProductType = productDetails.ProductType,
                    Images = productDetails.ProductImages?
                        .Select(img => new CatalogProductImageDto
                        {
                            Name = img.Name,
                            ImageData = img.ImageData,
                            IsMain = img.IsMain
                        }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving product details for catalog {catalogSlug}, product {productSlug}.", catalogSlug, productSlug);
                return null;
            }
        }

        /// <summary>
        /// Retrieves catalog information by its slug, including products.
        /// </summary>
        /// <param name="catalogSlug">The slug of the catalog.</param>
        /// <returns>A <see cref="CatalogDto"/> with catalog details and products.</returns>
        public async Task<CatalogDto> GetCatalogBySlugAsync(string catalogSlug)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving catalog by slug {catalogSlug}.", catalogSlug);
                return new CatalogDto();
            }
        }
    }
}
