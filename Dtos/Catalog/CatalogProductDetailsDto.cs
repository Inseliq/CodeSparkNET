
namespace CodeSparkNET.Dtos.Catalog
{
    public class CatalogProductDetailsDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public string? FullDescription { get; set; }
        public decimal Price { get; set; }
        public string? Currency { get; set; }
        public int InStock { get; set; }
        public string ProductType { get; set; }
        public bool IsAlreadyEnrolled { get; set; }
        public bool HasPrice { get; set; }
        public List<CatalogProductImageDto>? Images { get; set; }
    }
}