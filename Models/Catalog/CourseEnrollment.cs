using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents the enrollment of a user in a course.
    /// </summary>
    public class CourseEnrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!; // PK
        public AppUser AppUser { get; set; } = null!; // Navigation property to AppUser

        public Guid CourseId { get; set; } // PK
        public CourseDetail Course { get; set; } = null!; // Navigation property to CourseDetail

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow; // Enrollment date
        public bool IsCompleted { get; set; } = false;

    }
}