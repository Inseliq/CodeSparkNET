using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Category;
using CodeSparkNET.Models;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Dtos.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public string ShortDescription { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public ProductType ProductType { get; set; }
        public string ProductTypeName => ProductType.ToString();
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Categories
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        // Computed properties
        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice > Price;
        public decimal DiscountPercentage => HasDiscount && OriginalPrice.HasValue
            ? Math.Round(((OriginalPrice.Value - Price) / OriginalPrice.Value) * 100, 0)
            : 0;
        public bool HasRating => AverageRating > 0;
        public string FormattedPrice => Price.ToString("C");
        public string FormattedOriginalPrice => OriginalPrice?.ToString("C") ?? "";
    }
}