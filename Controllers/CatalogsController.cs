using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Controllers
{
  public class CatalogsController : Controller
  {
        private readonly ILogger<CatalogsController> _logger;
        private readonly ICatalogService _catalogService;
        public CatalogsController(ILogger<CatalogsController> logger, ICatalogService catalogService)
        {
            _logger = logger;
            _catalogService = catalogService;
        }
        [HttpGet("/Catalogs/Catalog/{catalogSlug}")]
        public async Task<IActionResult> Catalog(string catalogSlug)
        {
            try
            {
                var catalog = await _catalogService.GetCatalogBySlugAsync(catalogSlug);

                var model = new CatalogDto
                {
                    Name = catalog?.Name,
                    Slug = catalog?.Slug,
                    IsVisible = catalog?.IsVisible,
                    Products = catalog?.Products
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while requestiong catalog {Slug} by slug", catalogSlug);
                return View();
            }
        }

        [HttpGet("/Catalog/ProductDetails/{catalogSlug}/{productSlug}")]
        public async Task<IActionResult> ProductDetails(string catalogSlug, string productSlug)
        {
            try
            {
                var catalog = await _catalogService.GetCatalogProductDetailsAsync(catalogSlug, productSlug);

                var model = new CatalogProductDetailsDto
                {
                    Name = catalog?.Name,
                    Slug = catalog?.Slug,
                    FullDescription = catalog?.FullDescription,
                    Price = catalog.Price,
                    Currency = catalog?.Currency,
                    InStock = catalog.InStock,
                    Images = catalog?.Images
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while gettin product details {CatalogSlug}-{ProductSlug}", catalogSlug, productSlug);
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}