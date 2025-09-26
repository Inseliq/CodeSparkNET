using System.Collections.Generic;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents a base product entity in the catalog.
    /// Contains common properties such as name, price, descriptions, stock,
    /// and relationships with catalog and product images.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Unique identifier of the product.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Display name of the product.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// URL-friendly identifier (slug) for the product.
        /// Typically used in routing.
        /// </summary>
        public string Slug { get; set; } = null!;

        /// <summary>
        /// Short summary of the product (used in previews or listings).
        /// </summary>
        public string? ShortDescription { get; set; }

        /// <summary>
        /// Full, detailed description of the product.
        /// </summary>
        public string? FullDescription { get; set; }

        /// <summary>
        /// Price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Currency code of the product price (default: RUB).
        /// </summary>
        public string Currency { get; set; } = "RUB";

        /// <summary>
        /// Indicates whether the product is published and visible in the catalog.
        /// </summary>
        public bool IsPublished { get; set; } = true;

        /// <summary>
        /// Number of items available in stock.
        /// </summary>
        public int InStock { get; set; } = 0;

        /// <summary>
        /// Foreign key referencing the catalog that contains this product.
        /// </summary>
        public string CatalogId { get; set; } = null!;

        /// <summary>
        /// Navigation property to the parent catalog.
        /// </summary>
        public Catalog Catalog { get; set; }
        public string ProductType { get; set; }

        /// <summary>
        /// Collection of images associated with the product.
        /// </summary>
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
