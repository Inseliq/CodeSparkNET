using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Catalog;
using CodeSparkNET.Dtos.Products;
using CodeSparkNET.Models;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Dtos
{
    /// <summary>
    /// Dto for displaying catalog with products and filters
    /// </summary>
    public class CatalogDto
    {
        public CatalogConfiguration CatalogSettings { get; set; } = null!;

        // Products in the catalog
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();

        // Available filter options
        public CatalogFilterOptionsDto FilterOptions { get; set; } = new CatalogFilterOptionsDto();

        // Pagination info
        public PaginationDto Pagination { get; set; } = new PaginationDto();

        // Active filters
        public CatalogFilterDto Filters { get; set; } = new CatalogFilterDto();

        // Current sort option
        public CatalogSortOption CurrentSort { get; set; }

        // Counts
        public int TotalProductsCount { get; set; }
        public int FilteredProductsCount { get; set; }

        // Helper properties
        public bool HasActiveFilters => Filters.HasAnyFilter;
    }
}