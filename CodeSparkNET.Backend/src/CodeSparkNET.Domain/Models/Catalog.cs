namespace CodeSparkNET.Domain.Models
{
    /// <summary>
    /// Represents a catalog that contains a collection of products.
    /// Used for grouping and organizing products by category.
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// Unique identifier of the catalog.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Display name of the catalog.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// URL-friendly identifier (slug) for the catalog.
        /// </summary>
        public string Slug { get; set; } = null!;

        /// <summary>
        /// Indicates whether the catalog is visible to users.
        /// </summary>
        public bool? IsVisible { get; set; }

        /// <summary>
        /// Indicates whether the catalog is represented only by a link (no products).
        /// </summary>
        public bool IsLinkOnly { get; set; }

        /// <summary>
        /// Represents the name of the Razor Page associated with this catalog, if any.
        /// </summary>
        public string? PageName { get; set; } = null!;

        /// <summary>
        /// Represents the controller name for the Razor Page associated with this catalog, if any.
        /// </summary>
        public string? PageController { get; set; } = null!;

        /// <summary>
        /// List of products that belong to this catalog.
        /// </summary>
        public List<Product>? Products { get; set; } = new List<Product>();
    }
}
