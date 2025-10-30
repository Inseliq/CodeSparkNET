namespace CodeSparkNET.Domain.Models
{
    public class LessonResource
    {
        public string Id { get; set; }
        public string LessonId { get; set; }
        public Lesson Lesson { get; set; }
        public string Url { get; set; } // link to media server
        public string ResourceType { get; set; } // video, img, pdf, etc.
        public string? Title { get; set; }
        public int Position { get; set; }
    }
}
