using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class ProductImage
    {
        public string Id { get; set; }
        public string ProductId { get; set; } = null!;
        public string? Name { get; set; }
        public byte[]? ImageData { get; set; }
        public bool IsMain { get; set; } = false;

        public Product Product { get; set; } = null!;
    }
}