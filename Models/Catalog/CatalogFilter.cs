using System;
using System.Collections.Generic;
using System.Linq;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents filter criteria for catalog products.
    /// This is not a database model, but a DTO for filtering logic.
    /// </summary>
    public class CatalogFilter
    {
        // Search query
        public string? SearchQuery { get; set; }

        // Filter by categories
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();

        // Filter by product types
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();

        // Price range filter
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Sorting
        public CatalogSortOption SortBy { get; set; } = CatalogSortOption.DateDesc;

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // Helper properties
        public int Skip => (Page - 1) * PageSize;
        public bool HasSearchQuery => !string.IsNullOrWhiteSpace(SearchQuery);
        public bool HasCategoryFilter => CategoryIds.Any();
        public bool HasProductTypeFilter => ProductTypes.Any();
        public bool HasPriceFilter => MinPrice.HasValue || MaxPrice.HasValue;
        public bool HasAnyFilter => HasSearchQuery || HasCategoryFilter ||
                                   HasProductTypeFilter || HasPriceFilter;
    }
}