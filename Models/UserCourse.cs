namespace CodeSparkNET.Models
{
    public class UserCourse
    {
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public string CourseSlug { get; set; }
        public Course Course { get; set; } = null!;

        public DateTime EnrolledAt { get; set; }
        public bool IsCompleted { get; set; }
    }
}
