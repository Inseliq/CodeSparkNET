namespace CodeSparkNET.Domain.Models
{
    public class Lesson
    {
        public string Id { get; set; }
        public string? ModuleId { get; set; }
        public CourseModule Module { get; set; }
        public string Title { get; set; }
        public string? Slug { get; set; }
        public string? Body { get; set; }
        public int Position { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFreePreview { get; set; }

        public List<LessonResource> Resources { get; set; } = new();
    }
}
