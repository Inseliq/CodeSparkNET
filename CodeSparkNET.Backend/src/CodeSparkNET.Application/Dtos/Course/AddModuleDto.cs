namespace CodeSparkNET.Application.Dtos.Course
{
    public class AddModuleDto
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string CourseSlug { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
    }
}
