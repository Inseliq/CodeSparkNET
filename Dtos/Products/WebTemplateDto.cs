using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Dtos.Products
{
    public class WebTemplateDto
    {
        public Product Product { get; set; }
        public WebTemplateDetail TemplateDetail { get; set; }
        public List<WebTemplateReview> Reviews { get; set; }
        public double AverageRating { get; set; }
        public int TotalOrders { get; set; }
        public bool HasUserOrdered { get; set; }
        public List<string> FeaturesList => TemplateDetail.Features.Split(',').ToList();
        public List<string> TechnologiesList => TemplateDetail.Technologies.Split(',').ToList();
    }
}