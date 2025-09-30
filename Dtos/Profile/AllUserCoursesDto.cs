namespace CodeSparkNET.Dtos.Profile
{
    /// <summary>
    /// Data transfer object representing a user's course with basic information and completion status.
    /// </summary>
    public class AllUserCoursesDto
    {
        /// <summary>
        /// Display name of the course.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// URL-friendly identifier (slug) for the course.
        /// </summary>
        public string? Slug { get; set; }

        /// <summary>
        /// Indicates whether the user has completed the course.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}