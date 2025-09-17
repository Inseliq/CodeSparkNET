using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Dtos.Catalog
{
    public class CatalogDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public bool? IsVisible { get; set; }
        public List<Product>? Products { get; set; }
    }
}