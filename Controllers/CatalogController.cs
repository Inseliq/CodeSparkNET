using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodeSparkNET.Data;
using CodeSparkNET.Models;
using CodeSparkNET.Models.Enum;
using CodeSparkNET.Dtos;
using System.Linq;
using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Dtos.Products;
using CodeSparkNET.Dtos.Category;

namespace CodeSparkNET.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _context;

        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Главная страница каталога
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(CatalogFilterDto filters)
        {
            // Получаем настройки каталога
            var catalogSettings = await _context.CatalogConfigurations.FirstOrDefaultAsync();

            // Строим запрос продуктов
            var query = _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Where(p => p.IsPublished);

            // Применяем фильтры
            query = ApplyFilters(query, filters);

            // Подсчитываем общее количество
            var totalCount = await query.CountAsync();

            // Применяем сортировку
            query = ApplySorting(query, filters.SortBy);

            // Применяем пагинацию
            var products = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            // Создаем ViewModel
            var viewModel = new CatalogDto
            {
                CatalogSettings = catalogSettings,
                Products = products.Select(MapToProductViewModel),
                Filters = filters,
                CurrentSort = filters.SortBy,
                TotalProductsCount = await _context.Products.CountAsync(p => p.IsPublished),
                FilteredProductsCount = totalCount,
                Pagination = CreatePaginationViewModel(filters.Page, filters.PageSize, totalCount),
                FilterOptions = await CreateFilterOptionsViewModel(),
            };

            // Обновляем статистику просмотров
            await _context.SaveChangesAsync();

            return View(viewModel);
        }

        /// <summary>
        /// AJAX-обновление каталога (для фильтрации без перезагрузки страницы)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Filter(CatalogFilterDto filters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Аналогичная логика как в Index, но возвращаем частичное представление
            var query = _context.Products
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Where(p => p.IsPublished);

            query = ApplyFilters(query, filters);
            var totalCount = await query.CountAsync();

            query = ApplySorting(query, filters.SortBy);

            var products = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var viewModel = new CatalogDto
            {
                Products = products.Select(MapToProductViewModel),
                Filters = filters,
                FilteredProductsCount = totalCount,
                Pagination = CreatePaginationViewModel(filters.Page, filters.PageSize, totalCount)
            };

            return PartialView("_ProductsList", viewModel);
        }

        /// <summary>
        /// Применение фильтров к запросу
        /// </summary>
        private IQueryable<Product> ApplyFilters(IQueryable<Product> query, CatalogFilterDto filters)
        {
            // Поиск по названию и описанию
            if (filters.HasSearchQuery)
            {
                var searchTerm = filters.SearchQuery!.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.ShortDescription.ToLower().Contains(searchTerm));
            }

            // Фильтр по категориям
            if (filters.HasCategoryFilter)
            {
                query = query.Where(p =>
                    p.ProductCategories.Any(pc => filters.CategoryIds.Contains(pc.CategoryId)));
            }

            // Фильтр по типу продукта
            if (filters.HasProductTypeFilter)
            {
                query = query.Where(p => filters.ProductTypes.Contains(p.ProductType));
            }

            // Ценовой фильтр
            if (filters.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filters.MinPrice.Value);
            }
            if (filters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filters.MaxPrice.Value);
            }

            return query;
        }

        /// <summary>
        /// Применение сортировки
        /// </summary>
        private IQueryable<Product> ApplySorting(IQueryable<Product> query, CatalogSortOption sortBy)
        {
            return sortBy switch
            {
                CatalogSortOption.DateDesc => query.OrderByDescending(p => p.CreatedAt),
                CatalogSortOption.DateAsc => query.OrderBy(p => p.CreatedAt),
                CatalogSortOption.PriceAsc => query.OrderBy(p => p.Price),
                CatalogSortOption.PriceDesc => query.OrderByDescending(p => p.Price),
                CatalogSortOption.NameAsc => query.OrderBy(p => p.Title),
                CatalogSortOption.NameDesc => query.OrderByDescending(p => p.Title),
                CatalogSortOption.RatingDesc => query.OrderByDescending(p => p.AverageRating),
                CatalogSortOption.PopularityDesc => query.OrderByDescending(p => p.ViewsCount),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };
        }

        /// <summary>
        /// Маппинг Product в ProductViewModel
        /// </summary>
        private ProductDto MapToProductViewModel(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Slug = product.Slug,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                ShortDescription = product.ShortDescription,
                ThumbnailUrl = product.ThumbnailUrl,
                ProductType = product.ProductType,
                AverageRating = product.AverageRating,
                ReviewsCount = product.ReviewsCount,
                CreatedAt = product.CreatedAt,
                Categories = product.ProductCategories.Select(pc => new CategoryDto
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name,
                    Slug = pc.Category.Slug
                }).ToList()
            };
        }

        /// <summary>
        /// Создание ViewModel для пагинации
        /// </summary>
        private PaginationDto CreatePaginationViewModel(int currentPage, int pageSize, int totalItems)
        {
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new PaginationDto
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = currentPage > 1,
                HasNextPage = currentPage < totalPages,
                PageNumbers = GeneratePageNumbers(currentPage, totalPages),
                StartPage = Math.Max(1, currentPage - 2),
                EndPage = Math.Min(totalPages, currentPage + 2)
            };
        }

        /// <summary>
        /// Генерация номеров страниц для пагинации
        /// </summary>
        private List<int> GeneratePageNumbers(int currentPage, int totalPages)
        {
            var pages = new List<int>();
            var start = Math.Max(1, currentPage - 2);
            var end = Math.Min(totalPages, currentPage + 2);

            for (int i = start; i <= end; i++)
            {
                pages.Add(i);
            }

            return pages;
        }

        /// <summary>
        /// Создание опций для фильтров
        /// </summary>
        private async Task<CatalogFilterOptionsDto> CreateFilterOptionsViewModel()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var prices = await _context.Products
                .Where(p => p.IsPublished)
                .Select(p => p.Price)
                .ToListAsync();

            return new CatalogFilterOptionsDto
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),

                ProductTypes = Enum.GetValues<ProductType>()
                    .Select(pt => new SelectListItem
                    {
                        Value = ((int)pt).ToString(),
                        Text = pt.ToString()
                    }).ToList(),

                SortOptions = Enum.GetValues<CatalogSortOption>()
                    .Select(so => new SelectListItem
                    {
                        Value = ((int)so).ToString(),
                        Text = GetSortOptionDisplayName(so)
                    }).ToList(),

                LayoutOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Сетка" },
                    new SelectListItem { Value = "2", Text = "Список" }
                },

                PageSizeOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "12", Text = "12" },
                    new SelectListItem { Value = "24", Text = "24" },
                    new SelectListItem { Value = "48", Text = "48" }
                },
            };
        }

        /// <summary>
        /// Получение отображаемого названия для опции сортировки
        /// </summary>
        private string GetSortOptionDisplayName(CatalogSortOption sortOption)
        {
            return sortOption switch
            {
                CatalogSortOption.DateDesc => "Сначала новые",
                CatalogSortOption.DateAsc => "Сначала старые",
                CatalogSortOption.PriceAsc => "По цене (возрастание)",
                CatalogSortOption.PriceDesc => "По цене (убывание)",
                CatalogSortOption.NameAsc => "По названию А-Я",
                CatalogSortOption.NameDesc => "По названию Я-А",
                CatalogSortOption.RatingDesc => "По рейтингу",
                CatalogSortOption.PopularityDesc => "По популярности",
                _ => "По умолчанию"
            };
        }
    }
}