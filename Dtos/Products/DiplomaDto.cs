using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Dtos.Products
{
    public class DiplomaDto
    {
        public Product Product { get; set; }
        public DiplomaDetail DiplomaDetail { get; set; }
        public bool CanUserOrder { get; set; }
        public bool HasUserOrdered { get; set; }
        public List<string> KeywordsList => DiplomaDetail.Keywords.Split(',').ToList();
    }
}