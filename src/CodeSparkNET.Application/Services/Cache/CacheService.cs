using CodeSparkNET.Application.Dtos.Catalog;
using CodeSparkNET.Application.Dtos.Course;
using CodeSparkNET.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CodeSparkNET.Application.Services.Cache
{
    /// <summary>
    /// Service for caching user data using Redis.
    /// Provides methods for caching, retrieving, and clearing user data in Redis.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly ILogger<CacheService> _logger;
        private readonly ICacheProvider _cacheProvider;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IProductRepository _productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging cache operations.</param>
        /// <param name="cacheProvider">Custom cache provider for interacting with Redis.</param>
        public CacheService(
            ILogger<CacheService> logger,
            ICacheProvider cacheProvider,
            ICatalogRepository catalogRepository,
            IProductRepository productRepository
            )
        {
            _logger = logger;
            _cacheProvider = cacheProvider;
            _catalogRepository = catalogRepository;
            _productRepository = productRepository;
        }

        private string GetCatalogKey(string slug) => $"catalog:{slug}:full";
        private string GetCourseKey(string slug) => $"course:{slug}:full";

        #region Catalog cache oprations
        public async Task CacheCatalogNamesAsync()
        {
            try
            {
                var key = $"catalog-names";
                var catalogs = await _catalogRepository.GetCatalogsAsync();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                };

                await _cacheProvider.SetCache(key, catalogs, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving catalogs name in cache");
            }
        }

        public async Task<List<CatalogNamesDto>> GetCachedCatalogNamesAsync()
        {
            try
            {
                var key = $"catalog-names";
                var cachedCatalogs = await _cacheProvider.GetFromCache<List<Catalog>>(key);

                if (cachedCatalogs != null)
                {
                    return cachedCatalogs
                            .Select(c => new CatalogNamesDto
                            {
                                Name = c.Name,
                                Slug = c.Slug,
                                IsLinkOnly = c.IsLinkOnly,
                                PageName = c.PageName,
                                PageController = c.PageController
                            }).ToList();
                }
                else
                {
                    var catalogs = await _catalogRepository.GetCatalogsAsync();
                    await CacheCatalogNamesAsync();
                    if (catalogs != null)
                    {
                        return catalogs
                                .Select(c => new CatalogNamesDto
                                {
                                    Name = c.Name,
                                    Slug = c.Slug
                                }).ToList();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting catalog from cache");
                return null;
            }
        }

        public async Task ClearCachedNamesAsync()
        {
            try
            {
                var key = $"catalog-names";

                await _cacheProvider.ClearCache(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting catalog names from cache");
            }
        }

        public async Task CacheCatalogBySlugAsync(string slug)
        {
            try
            {
                if (string.IsNullOrEmpty(slug))
                    return;

                var key = GetCatalogKey(slug);

                var catalog = await _catalogRepository.GetCatalogBySlugAsync(slug);

                var catalogDto = new CatalogDto
                {
                    Name = catalog.Name,
                    Slug = catalog.Slug,
                    IsVisible = catalog.IsVisible,
                    IsLinkOnly = catalog.IsLinkOnly,
                    PageController = catalog.PageController,
                    PageName = catalog.PageName,
                    Products = catalog.Products.Select(p => new CatalogProductsDto
                    {
                        Name = p.Name,
                        Slug = p.Slug,
                        ShortDescription = p.ShortDescription,
                        FullDescription = p.FullDescription,
                        Price = p.Price,
                        Currency = p.Currency,
                        InStock = p.InStock,
                        ProductType = p.ProductType,
                        HasPrice = p.Price != 0m,
                        Group = p.Group,
                        ProductImages = p.ProductImages?.Select(img => new CatalogProductImageDto
                        {
                            Name = img.Name,
                            Url = img.Url,
                            IsMain = img.IsMain
                        }).ToList()
                    }).ToList()
                };

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };

                await _cacheProvider.SetCache(key, catalogDto, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting catalog in cache");
            }
        }

        public async Task<CatalogDto> GetCachedCatalogBySlugAsync(string slug)
        {
            try
            {
                var key = GetCatalogKey(slug);

                var cachedCatalog = await _cacheProvider.GetFromCache<CatalogDto>(key);

                if (cachedCatalog != null)
                {
                    return cachedCatalog;
                }
                else
                {
                    var catalog = await _catalogRepository.GetCatalogBySlugAsync(slug);

                    if (catalog != null)
                    {
                        await CacheCatalogBySlugAsync(slug);
                        return new CatalogDto
                        {
                            Name = catalog.Name,
                            Slug  = catalog.Slug,
                            IsVisible = catalog.IsVisible,
                            IsLinkOnly = catalog.IsLinkOnly,
                            PageController = catalog.PageController,
                            PageName = catalog.PageName,
                            Products = catalog.Products.Select(p => new CatalogProductsDto
                            {
                                Name = p.Name,
                                Slug = p.Slug,
                                ShortDescription = p.ShortDescription,
                                FullDescription = p.FullDescription,
                                Price = p.Price,
                                Currency = p.Currency,
                                InStock = p.InStock,
                                ProductType = p.ProductType,
                                HasPrice = p.Price != 0m,
                                Group = p.Group,
                                ProductImages = p.ProductImages?.Select(img => new CatalogProductImageDto
                                {
                                    Name = img.Name,
                                    Url = img.Url,
                                    IsMain = img.IsMain
                                }).ToList()
                            }).ToList()
                        };
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get catalog from cache");
                return null;
            }
        }

        public async Task ClearCachedCatalogBySlugAsync(string slug)
        {
            try
            {
                var key = GetCatalogKey(slug);

                await _cacheProvider.ClearCache(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting catalog fom cache");
            }
        }

        public async Task CacheCourseBySlugAsync(string slug)
        {
            try
            {
                if (string.IsNullOrEmpty(slug))
                    return;
                var key = GetCourseKey(slug);
                var course = await _productRepository.GetCourseBySlugAsync(slug);
                if (course == null)
                    return;

                var courseDto = new CourseDto
                {
                    Id = course.Id,
                    Name = course.Name,
                    Slug = course.Slug,
                    ShortDescription = course.ShortDescription,
                    Images = course.ProductImages
                        .OrderBy(pi => pi.Position)
                        .Select(pi => new ProductImageDto
                        {
                            Url = pi.Url ?? pi.Name ?? "",
                            AltText = pi.AltText,
                            IsMain = pi.IsMain,
                            Name = pi.Name ?? ""
                        })
                        .ToList(),
                    Modules = course.Modules
                        .OrderBy(m => m.Position)
                        .Select(m => new ModuleDto
                        {
                            Id = m.Id,
                            Title = m.Title,
                            Position = m.Position,
                            Lessons = m.Lessons
                                .OrderBy(l => l.Position)
                                .Select(l => new LessonListItemDto
                                {
                                    Id = l.Id,
                                    Title = l.Title,
                                    Position = l.Position,
                                    Slug = l.Slug,
                                    IsFreePreview = l.IsFreePreview,
                                })
                                .ToList()
                        }).ToList()
                };

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };
                await _cacheProvider.SetCache(key, courseDto, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting course in cache");
            }
        }

        public async Task<CourseDto> GetCachedCourseBySlugAsync(string slug)
        {
            try
            {
                var key = GetCourseKey(slug);
                var cachedCourse = await _cacheProvider.GetFromCache<CourseDto>(key);
                if (cachedCourse != null)
                {
                    return cachedCourse;
                }
                else
                {
                    var course = await _productRepository.GetCourseBySlugAsync(slug);
                    if (course != null)
                    {
                        await CacheCourseBySlugAsync(slug);
                        return cachedCourse;
                    }
                    return new CourseDto();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get course from cache");
                return new CourseDto();
            }
        }

        public async Task ClearCachedCourseBySlugAsync(string slug)
        {
            try
            {
                var key = GetCourseKey(slug);
                await _cacheProvider.ClearCache(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course fom cache");
            }
        }
        #endregion
    }
}
