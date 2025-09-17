using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Models
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public bool EmailMarketingConsent { get; set; }

        public ICollection<UserCourse> UserCourses { get; set; }

    }
}