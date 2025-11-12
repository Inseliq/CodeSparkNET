namespace CodeSparkNET.Application.Dtos.Catalog
{
    public class CatalogDto
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public List<CatalogProductsDto>? Products { get; set; }
    }
}