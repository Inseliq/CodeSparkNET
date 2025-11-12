namespace CodeSparkNET.WEB.ViewModels.Catalogs
{
    public class CatalogViewModel
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }

        public List<CatalogProductsViewModel>? Products { get; set; }
    }
}
