namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents an educational course that extends the base Product entity.
    /// Includes additional course-specific details such as difficulty level
    /// and the relationship with users enrolled in the course.
    /// </summary>
    public class Course : Product
    {
        /// <summary>
        /// Difficulty level of the course (e.g., Beginner, Intermediate, Advanced).
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// Navigation property representing users enrolled in this course.
        /// </summary>
        public ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}
