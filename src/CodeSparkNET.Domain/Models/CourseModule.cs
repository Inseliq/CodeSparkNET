namespace CodeSparkNET.Domain.Models
{
    public class CourseModule
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string? CourseId { get; set; }
        public Course? Course { get; set; }
        public string? Title { get; set; }
        public int Position { get; set; }
        public List<Lesson> Lessons { get; set; } = new();

    }
}
