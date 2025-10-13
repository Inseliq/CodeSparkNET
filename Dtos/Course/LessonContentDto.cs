using CodeSparkNET.Models;

namespace CodeSparkNET.Dtos.Course
{
    public class LessonContentDto
    {
        public string Id { get; set; }
        public string? Slug { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Position { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFreePreview { get; set; }
        public List<LessonResourceDto> Resources { get; set; } = new();
    }
}
