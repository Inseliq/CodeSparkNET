namespace CodeSparkNET.Models
{
    public class CourseModule
    {
        public string Id { get; set; }
        public string? CourseId { get; set; }
        public Course? Course { get; set; }
        public string? Title { get; set; }
        public int Position { get; set; }
        public List<Lesson> Lessons { get; set; } = new();

    }
}
