using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Dtos.User;
using CodeSparkNET.Interfaces.Repositories;
using CodeSparkNET.Interfaces.Services;
using CodeSparkNET.Models;
using CodeSparkNET.Redis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace CodeSparkNET.Services
{
    /// <summary>
    /// Service for caching user data using Redis.
    /// Provides methods for caching, retrieving, and clearing user data in Redis.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly ILogger<CacheService> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICacheProvider _cacheProvider;
        private readonly ICatalogRepository _catalogRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging cache operations.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        /// <param name="cacheProvider">Custom cache provider for interacting with Redis.</param>
        public CacheService(
            ILogger<CacheService> logger,
            UserManager<AppUser> userManager,
            ICacheProvider cacheProvider,
            ICatalogRepository catalogRepository
            )
        {
            _logger = logger;
            _userManager = userManager;
            _cacheProvider = cacheProvider;
            _catalogRepository = catalogRepository;
        }

        private string GetCatalogKey(string slug) => $"catalog:{slug}:full";
        private string GetCourseKey(string slug) => $"course:{slug}:full";

        #region User cache oprations

        /// <summary>
        /// Caches a user in Redis by their email.
        /// </summary>
        /// <param name="email">The email of the user to cache.</param>
        public async Task CacheUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return;

                var roles = await _userManager.GetRolesAsync(user);

                var model = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = roles ?? new List<string>()
                };

                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30),
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
                };

                await _cacheProvider.SetCache(email, model, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching user data for email - {email}", email);
            }
        }

        /// <summary>
        /// Retrieves a cached user from Redis by their email.
        /// If not found in cache, retrieves the user from the database and caches them.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>The <see cref="AppUser"/> instance if found, otherwise null.</returns>
        public async Task<UserDto> GetCachedUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            try
            {
                var cachedUser = await _cacheProvider.GetFromCache<UserDto>(email);
                if (cachedUser != null)
                {
                    return new UserDto
                    {
                        Id = cachedUser.Id,
                        Email = cachedUser.Email,
                        UserName = cachedUser.UserName,
                        Roles = cachedUser.Roles
                    };
                }
                else
                {
                    // If not in cache, get from database and cache it
                    var user = await _userManager.FindByEmailAsync(email);
                    var roles = await _userManager.GetRolesAsync(user);
                    if (user != null)
                    {
                        await CacheUserAsync(email);
                        return new UserDto
                        {
                            Id = user.Id,
                            Email = user.Email,
                            UserName = user.UserName,
                            Roles = roles
                        };
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached user data for email - {email}", email);
                return null;
            }
        }

        /// <summary>
        /// Clears a cached user entry from Redis by their email.
        /// </summary>
        /// <param name="email">The email of the user to clear from cache.</param>
        public async Task ClearCachedUserAsync(string email)
        {
            await _cacheProvider.ClearCache(email);
        }
        #endregion

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
                                Slug = c.Slug
                            }).ToList();
                }
                else
                {
                    var catalogs = await _catalogRepository.GetCatalogsAsync();

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

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                };

                await _cacheProvider.SetCache(key, catalog, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting catalog in cache");
            }
        }

        public async Task<Catalog> GetCachedCatalogBySlugAsync(string slug)
        {
            try
            {
                var key = GetCatalogKey(slug);

                var cachedCatalog = await _cacheProvider.GetFromCache<Catalog>(key);

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
                        return catalog;
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
        #endregion
    }
}
