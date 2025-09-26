namespace CodeSparkNET.Dtos.Course
{
    public class LessonContentDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<LessonResourceDto> Resources { get; set; } = new();
    }
}
