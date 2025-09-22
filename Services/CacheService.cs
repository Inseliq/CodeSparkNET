using CodeSparkNET.Dtos.User;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging cache operations.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        /// <param name="cacheProvider">Custom cache provider for interacting with Redis.</param>
        public CacheService(
            ILogger<CacheService> logger,
            UserManager<AppUser> userManager,
            ICacheProvider cacheProvider
            )
        {
            _logger = logger;
            _userManager = userManager;
            _cacheProvider = cacheProvider;
        }

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

                // get roles if you need role in DTO
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles?.FirstOrDefault();

                // map to safe DTO — do NOT include PasswordHash or security stamps
                var model = new UserDto
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = role
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
        public async Task<AppUser> GetCachedUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            try
            {
                var cachedUser = await _cacheProvider.GetFromCache<UserDto>(email);
                if (cachedUser != null)
                {
                    return new AppUser
                    {
                        Email = cachedUser.Email,
                        UserName = cachedUser.UserName
                    };
                }
                else
                {
                    // If not in cache, get from database and cache it
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        await CacheUserAsync(email);
                        return user;
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
    }
}
