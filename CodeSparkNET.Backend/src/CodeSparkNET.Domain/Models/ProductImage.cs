namespace CodeSparkNET.Domain.Models
{
    /// <summary>
    /// Represents an image associated with a product.
    /// Stores URL to the image (hosted on CDN/blob/static) and metadata.
    /// </summary>
    public class ProductImage
    {
        /// <summary>
        /// Unique identifier of the product image.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Foreign key referencing the related product.
        /// </summary>
        public string ProductId { get; set; } = null!;

        /// <summary>
        /// Optional name or label for the image (e.g., original file name).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Publicly accessible URL to the image (CDN/Blob/etc).
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// Alternative text for picture
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Indicates whether this image is the main (primary) image of the product.
        /// </summary>
        public bool IsMain { get; set; } = false;

        /// <summary>
        /// Ordering position for images.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Navigation property to the related product.
        /// </summary>
        public Product Product { get; set; } = null!;
    }
}
