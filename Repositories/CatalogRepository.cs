using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Data;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly AppDbContext _context;
        public CatalogRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Catalog>> GetCatalogsAsync()
        {
            return await _context.Catalogs
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Catalog> GetCatalogBySlugAsync(string catalogSlug)
        {
            return await _context.Catalogs
                .AsNoTracking()
                .Where(c => c.Slug == catalogSlug)
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync();
        }
    }
}