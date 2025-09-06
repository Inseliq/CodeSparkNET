using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class CatalogConfiguration
    {
        [Key]
        public int Id { get; set; } = 1; // Singleton pattern

        // Settings for pagination and display
        public int ProductsPerPage { get; set; } = 12;
        public int FeaturedProductsCount { get; set; } = 8;
        public int PopularProductsCount { get; set; } = 6;
        public int RecentProductsCount { get; set; } = 6;

        // Filter settings
        public bool EnablePriceFilter { get; set; } = true;
        public bool EnableCategoryFilter { get; set; } = true;
        public bool EnableProductTypeFilter { get; set; } = true;
        public bool EnableRatingFilter { get; set; } = true;

        // Sort settings
        public bool EnableSortByPrice { get; set; } = true;
        public bool EnableSortByDate { get; set; } = true;
        public bool EnableSortByRating { get; set; } = true;
        public bool EnableSortByPopularity { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}