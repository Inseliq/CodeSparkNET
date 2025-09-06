using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Models
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Primary key
        public string Title { get; set; } = null!; // Title of the product
        public string Slug { get; set; } = null!; // URL-friendly version of the title
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // Price in RUB
        public string ShortDescription { get; set; } = null!; // Brief description of the product
        public string ThumbnailUrl { get; set; } = null!; // URL to the product thumbnail image
        public ProductType ProductType { get; set; } // enum Course = 1, Template = 2, Diploma = 3
        public bool IsPublished { get; set; } // Whether the product is published or not
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Set default to current UTC time

    }
}