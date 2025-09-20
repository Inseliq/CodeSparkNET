namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents the enrollment of a user in a specific course.
    /// Tracks the course, the user, enrollment date, and completion status.
    /// </summary>
    public class UserCourse
    {
        /// <summary>
        /// Unique identifier for the user-course relationship.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Foreign key referencing the enrolled user.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Navigation property to the enrolled user.
        /// </summary>
        public AppUser User { get; set; } = null!;

        /// <summary>
        /// Slug of the course the user is enrolled in.
        /// </summary>
        public string CourseSlug { get; set; }

        /// <summary>
        /// Navigation property to the related course.
        /// </summary>
        public Course Course { get; set; } = null!;

        /// <summary>
        /// Date and time when the user enrolled in the course.
        /// </summary>
        public DateTime EnrolledAt { get; set; }

        /// <summary>
        /// Indicates whether the user has completed the course.
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
