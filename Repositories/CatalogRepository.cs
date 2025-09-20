using CodeSparkNET.Data;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Repositories
{
    /// <summary>
    /// Repository responsible for operations on the <see cref="Catalog"/> entity.
    /// Provides methods to retrieve all catalogs and to retrieve a single catalog by its slug.
    /// </summary>
    public class CatalogRepository : ICatalogRepository
    {
        /// <summary>
        /// Application database context used for data access.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Logger for the repository.
        /// </summary>
        private readonly ILogger<CatalogRepository> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="CatalogRepository"/>.
        /// </summary>
        /// <param name="context">The <see cref="AppDbContext"/> instance for database access.</param>
        /// <param name="logger">The logger instance for recording information and errors.</param>
        public CatalogRepository(AppDbContext context, ILogger<CatalogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously returns a list of all catalogs from the database.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="Microsoft.EntityFrameworkCore.Query.IQueryableExtensions.AsNoTracking{TResult}"/>
        /// to disable change tracking for better read performance. On error the exception is logged and an empty list is returned.
        /// </remarks>
        /// <returns>
        /// A <see cref="System.Collections.Generic.List{Catalog}"/> containing all catalogs. If an error occurs, returns an empty list.
        /// </returns>
        /// <exception cref="System.Exception">All exceptions are logged and swallowed; they are not rethrown by this method.</exception>
        public async Task<List<Catalog>> GetCatalogsAsync()
        {
            try
            {
                return await _context.Catalogs
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error with getting all catalogs from Database");
                return new List<Catalog>();
            }
        }

        /// <summary>
        /// Asynchronously retrieves a catalog by its slug, including its products and each product's images.
        /// </summary>
        /// <remarks>
        /// The query uses <see cref="AsNoTracking"/> and includes navigation properties:
        /// <see cref="Catalog.Products"/> and <see cref="Product.ProductImages"/> via <see cref="Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include{TEntity, TProperty}"/>.
        /// If no catalog matches the provided slug, <c>null</c> will be returned by <see cref="FirstOrDefaultAsync"/>. In the current implementation,
        /// if an exception occurs the error is logged and a new empty <see cref="Catalog"/> instance is returned.
        /// </remarks>
        /// <param name="catalogSlug">The slug of the catalog to find (for example: "electronics").</param>
        /// <returns>
        /// The matching <see cref="Catalog"/> including its products and product images; if not found, the result of <c>FirstOrDefaultAsync()</c> (typically <c>null</c>).
        /// On error, a new empty <see cref="Catalog"/> instance is returned.
        /// </returns>
        /// <exception cref="System.Exception">All exceptions are logged and swallowed; they are not rethrown by this method.</exception>
        public async Task<Catalog> GetCatalogBySlugAsync(string catalogSlug)
        {
            try
            {
                return await _context.Catalogs
                   .AsNoTracking()
                   .Where(c => c.Slug == catalogSlug)
                   .Include(c => c.Products)
                   .ThenInclude(p => p.ProductImages)
                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting catalog by slug {Slug}", catalogSlug);
                return new Catalog();
            }
        }
    }
}
