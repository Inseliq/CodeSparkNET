using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Dtos.Catalog
{
    public class CatalogProductsDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public int InStock { get; set; }
        public byte[]? Image { get; set; }
    }
}