using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CodeSparkNET.Dtos.User;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using CodeSparkNET.Redis;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace CodeSparkNET.Services
{
    /// <summary>
    /// Service for caching user data using Redis.
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly ILogger<CacheService> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICacheProvider _cacheProvider;
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

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка кеширования данных пользователя с почтой - {email}", email);
                return;
            }
        }

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
                _logger.LogError(ex, "Ошибка получения кешированных данных пользователя с почтой - {email}", email);
                return null;
            }
        }

        public async Task ClearCachedUserAsync(string email)
        {
            await _cacheProvider.ClearCache(email);
        }
    }
}