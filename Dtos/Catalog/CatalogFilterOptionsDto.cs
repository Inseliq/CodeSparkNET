using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CodeSparkNET.Dtos.Catalog
{
    /// <summary>
    /// ViewModel for filter options (dropdowns, ranges, etc.)
    /// </summary>
    public class CatalogFilterOptionsDto
    {
        // Categories available for filtering
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        // Product types available for filtering
        public List<SelectListItem> ProductTypes { get; set; } = new List<SelectListItem>();

        // Sort options
        public List<SelectListItem> SortOptions { get; set; } = new List<SelectListItem>();

        // Layout options (grid/list)
        public List<SelectListItem> LayoutOptions { get; set; } = new List<SelectListItem>();

        // Page size options
        public List<SelectListItem> PageSizeOptions { get; set; } = new List<SelectListItem>();

    }
}