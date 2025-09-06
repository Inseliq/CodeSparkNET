using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Dtos.Catalog
{
    /// <summary>
    /// Dto for catalog filters - used for form binding
    /// </summary>
    public class CatalogFilterDto
    {
        [Display(Name = "Поиск")]
        public string? SearchQuery { get; set; }

        [Display(Name = "Категории")]
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();

        [Display(Name = "Тип товара")]
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();

        [Display(Name = "Цена от")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительной")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Цена до")]
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительной")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Только со скидкой")]
        public bool? HasDiscount { get; set; }


        [Display(Name = "Сортировка")]
        public CatalogSortOption SortBy { get; set; } = CatalogSortOption.DateDesc;

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Computed properties
        public bool HasSearchQuery => !string.IsNullOrWhiteSpace(SearchQuery);
        public bool HasCategoryFilter => CategoryIds.Any();
        public bool HasProductTypeFilter => ProductTypes.Any();
        public bool HasPriceFilter => MinPrice.HasValue || MaxPrice.HasValue;
        public bool HasAnyFilter => HasSearchQuery || HasCategoryFilter ||
                                    HasProductTypeFilter || HasPriceFilter
                                    || HasDiscount == true;
    }
}