using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }

        public decimal Price { get; set; }
        public string Currency { get; set; } = "RUB";
        public bool IsPublished { get; set; } = true;
        public int InStock { get; set; } = 0;

        public string CatalogId { get; set; } = null!;
        public Catalog Catalog { get; set; }

        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}