using CodeSparkNET.Models;

namespace CodeSparkNET.Dtos.Course
{
    public class UpdateModuleDto
    {
        public string Slug { get; set; }
        public string? Title { get; set; }
        public int Position { get; set; }
    }
}
