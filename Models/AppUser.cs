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

        // Навигационные свойства для дипломных работ
        public ICollection<DiplomaOrder> DiplomaOrders { get; set; } = new List<DiplomaOrder>();

        // Навигационные свойства для веб-шаблонов
        public ICollection<WebTemplateOrder> TemplateOrders { get; set; } = new List<WebTemplateOrder>();
        public ICollection<WebTemplateReview> TemplateReviews { get; set; } = new List<WebTemplateReview>();
        public bool EmailMarketingConsent { get; set; }
    }
}