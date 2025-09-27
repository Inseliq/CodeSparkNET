using CodeSparkNET.Dtos.Course;

namespace CodeSparkNET.Dtos.Catalog
{
    public class CatalogProductsDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public int InStock { get; set; }
        public string ProductType { get; set; }
        public List<CatalogProductImageDto> ProductImages { get; set; }
    }
}