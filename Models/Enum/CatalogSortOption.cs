namespace CodeSparkNET.Models.Enum
{
    public enum CatalogSortOption
    {
        DateDesc = 1,       // Date (newest first)
        DateAsc = 2,        // Date (oldest first)
        PriceAsc = 3,       // Price (ascending)
        PriceDesc = 4,      // Price (descending)
        NameAsc = 5,        // Name A-Я
        NameDesc = 6,       // Name Я-А
        RatingDesc = 7,     // Rating (highest first)
        PopularityDesc = 8  // Popularity (most purchased first)
    }
}