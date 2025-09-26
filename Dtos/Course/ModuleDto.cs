namespace CodeSparkNET.Dtos.Course
{
    public class ModuleDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public List<LessonListItemDto> Lessons { get; set; } = new();
    }
}
