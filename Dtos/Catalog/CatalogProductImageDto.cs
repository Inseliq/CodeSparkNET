using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Dtos.Catalog
{
    public class CatalogProductImageDto
    {
        public string? Name { get; set; }
        public string Image { get; set; }
        public bool? IsMain { get; set; }
    }
}