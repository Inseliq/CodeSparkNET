using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Dtos.Catalog
{
    public class PaginationDto
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public List<int> PageNumbers { get; set; } = new List<int>();
        public int StartPage { get; set; }
        public int EndPage { get; set; }
    }
}