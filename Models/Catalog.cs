using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class Catalog
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public bool? IsVisible { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

    }
}