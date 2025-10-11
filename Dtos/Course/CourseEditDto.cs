namespace CodeSparkNET.Dtos.Course
{
    public class CourseEditDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Price { get; set; } = 0m;
        public string Slug { get; set; } = null!;
        public int InStock { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public string? MainImageUrl { get; set; }

        public List<ModuleEditDto> Modules { get; set; } = new();

    }
}
