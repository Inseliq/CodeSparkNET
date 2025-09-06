using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents the many-to-many relationship between products and categories.
    /// </summary>
    public class ProductCategory
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Additional properties
        public bool IsPrimary { get; set; } = false; // Основная категория для продукта
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Entity Framework configuration
        public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
        {
            public void Configure(EntityTypeBuilder<ProductCategory> builder)
            {
                builder.HasKey(pc => new { pc.ProductId, pc.CategoryId });

                builder.HasOne(pc => pc.Product)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(pc => pc.ProductId);

                builder.HasOne(pc => pc.Category)
                    .WithMany(c => c.ProductCategories)
                    .HasForeignKey(pc => pc.CategoryId);
            }
        }
    }
}