using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class UserAssignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid AssignmentId { get; set; }
        public CourseAssignment CourseAssignment { get; set; } = null!; // Navigation property

        public string UserId { get; set; } = null!; // FK
        public AppUser AppUser { get; set; } = null!; // Navigation property

        public string? AnswerText { get; set; }
        public string? FileUrl { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}