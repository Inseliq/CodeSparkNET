using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Services
{
    public interface ICacheService
    {
        Task CacheUserAsync(string email);
        Task<AppUser> GetCachedUserAsync(string email);
        Task ClearCachedUserAsync(string email);
    }
}