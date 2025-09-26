using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Interfaces.Services;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeSparkNET.Controllers
{
  public class CatalogsController : Controller
  {
        private readonly ILogger<CatalogsController> _logger;
        private readonly ICatalogService _catalogService;
        private readonly IAccountService _accountService;
        private readonly ICacheService _cacheService;
        public CatalogsController(
            ILogger<CatalogsController> logger,
            ICatalogService catalogService,
            IAccountService accountService,
            ICacheService cacheService)
        {
            _logger = logger;
            _catalogService = catalogService;
            _accountService = accountService;
            _cacheService = cacheService;
        }

        public IActionResult MiniApp()
        {
            return View();
        }

        [HttpGet("/media/{fileName}")]
        public IActionResult GetVideo(string fileName)
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/assets/video",
                fileName
            );

            if (!System.IO.File.Exists(path))
                return NotFound();

            return PhysicalFile(path, "video/mp4", enableRangeProcessing: true);
        }

        public async Task<IActionResult> Catalogs()
        {
            try
            {
                var catalogs = await _catalogService.GetCatalogNamesAsync();
                return View(catalogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while requestiong catalogs");
                return View();
            }
        }

        [HttpGet("/Catalogs/Catalog/{catalogSlug}")]
        public async Task<IActionResult> Catalog(string catalogSlug)
        {
            try
            {
                var catalog = await _cacheService.GetCachedCatalogBySlugAsync(catalogSlug);

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

        [Authorize]
        [HttpGet("/Catalog/ProductDetails/{catalogSlug}/{productSlug}")]
        public async Task<IActionResult> ProductDetails(string catalogSlug, string productSlug)
        {
            try
            {
                var catalog = await _catalogService.GetCatalogProductDetailsAsync(catalogSlug, productSlug);
                var cachedUserDto = await _cacheService.GetCachedUserAsync(User.FindFirstValue(ClaimTypes.Email));

                var model = new CatalogProductDetailsDto
                {
                    Name = catalog?.Name,
                    Slug = catalog?.Slug,
                    FullDescription = catalog?.FullDescription,
                    Price = catalog.Price,
                    Currency = catalog?.Currency,
                    InStock = catalog.InStock,
                    Images = catalog?.Images,
                    IsAlreadyEnrolled = await _accountService.IsCourseAlreadyEnrolled(cachedUserDto.Id, productSlug),
                    ProductType = catalog?.ProductType
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while gettin product details {CatalogSlug}-{ProductSlug}", catalogSlug, productSlug);
                return View();
            }
        }

        [HttpGet("/Catalogs/AddCourseToUser/{productSlug}")]
        public async Task<IActionResult> AddCourseToUser(string productSlug)
        {
            try
            {
                var cachedUserDto = await _cacheService.GetCachedUserAsync(User.FindFirstValue(ClaimTypes.Email));

                var result = await _accountService.AddCourseToUserAsync(cachedUserDto.Id, productSlug);
                if (result)
                {
                    return Json(new {success = true, message = "���� ������� ��������."});
                }
                else
                {
                    return Json(new {success = false, message = "���� ��� ��������."});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding course {ProductSlug} to user", productSlug);
                return View("Error!");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}