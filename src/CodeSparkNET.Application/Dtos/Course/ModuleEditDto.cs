namespace CodeSparkNET.Application.Dtos.Course
{
    public class ModuleEditDto
    {
        public string Id { get; set; } = null!;
        public string Slug { get; set; }
        public string Title { get; set; } = null!;
        public int Position { get; set; }
        public List<LessonEditDto> Lessons { get; set; } = new();
    }
}
