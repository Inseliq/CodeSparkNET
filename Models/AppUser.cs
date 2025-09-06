using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Навигация
        public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
        public ICollection<UserAssignment> Assignments { get; set; } = new List<UserAssignment>();
        public bool EmailMarketingConsent { get; set; }
    }
}