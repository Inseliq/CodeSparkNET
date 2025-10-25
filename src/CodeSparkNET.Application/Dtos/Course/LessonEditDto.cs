namespace CodeSparkNET.Application.Dtos.Course
{
    public class LessonEditDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Body { get; set; }
        public int Position { get; set; }
    }
}
