using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Interfaces.Repositories
{
    public interface ICatalogRepository
    {
        Task<List<Catalog>> GetCatalogsAsync();
        Task<Catalog> GetCatalogBySlugAsync(string catalogSlug);
    }
}