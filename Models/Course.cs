namespace CodeSparkNET.Models
{
    public class Course : Product
    {
        public string Level { get; set; }
        public ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();

    }
}
