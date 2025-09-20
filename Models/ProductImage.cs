using System;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents an image associated with a product.
    /// Stores binary image data and indicates whether it is the main product image.
    /// </summary>
    public class ProductImage
    {
        /// <summary>
        /// Unique identifier of the product image.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Foreign key referencing the related product.
        /// </summary>
        public string ProductId { get; set; } = null!;

        /// <summary>
        /// Optional name or label for the image (e.g., file name or description).
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Binary data of the image.
        /// </summary>
        public byte[]? ImageData { get; set; }

        /// <summary>
        /// Indicates whether this image is the main (primary) image of the product.
        /// </summary>
        public bool IsMain { get; set; } = false;

        /// <summary>
        /// Navigation property to the related product.
        /// </summary>
        public Product Product { get; set; } = null!;
    }
}
